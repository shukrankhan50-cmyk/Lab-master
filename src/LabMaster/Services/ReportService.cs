using LabMaster.Data;
using Microsoft.Data.SqlClient;

namespace LabMaster.Services;

public sealed class ReportService
{
    public async Task<List<ReportOrder>> GetEnteredOrdersAsync()
    {
        await using var c = Database.CreateConnection();
        await c.OpenAsync();
        const string sql = "SELECT DISTINCT o.OrderId,o.OrderNumber,o.ReportNumber,p.MRNumber,p.PatientName,p.Gender,o.OrderDate,o.Status FROM dbo.TestOrders o JOIN dbo.Patients p ON p.PatientId=o.PatientId JOIN dbo.TestOrderItems oi ON oi.OrderId=o.OrderId WHERE oi.Status IN ('Entered','Verified') ORDER BY o.OrderId DESC";
        await using var cmd = new SqlCommand(sql, c);
        await using var r = await cmd.ExecuteReaderAsync();
        var list = new List<ReportOrder>();
        while (await r.ReadAsync())
            list.Add(new ReportOrder(r.GetInt32(0), r.GetString(1), r.IsDBNull(2) ? null : r.GetString(2), r.GetString(3), r.GetString(4), r.GetString(5), r.GetDateTime(6), r.GetString(7)));
        return list;
    }

    public async Task<ReportData?> GetReportAsync(int orderId)
    {
        await using var c = Database.CreateConnection();
        await c.OpenAsync();
        const string sql = "SELECT o.OrderNumber,o.ReportNumber,p.MRNumber,p.PatientName,p.Gender,p.Phone,o.OrderDate,o.VerifiedBy,o.VerifiedAt,o.ReportIssuedAt,t.TestCode,t.TestName,d.DepartmentName,t.Unit,t.ReferenceRange,oi.ResultValue,tr.ResultComment FROM dbo.TestOrders o JOIN dbo.Patients p ON p.PatientId=o.PatientId JOIN dbo.TestOrderItems oi ON oi.OrderId=o.OrderId JOIN dbo.Tests t ON t.TestId=oi.TestId JOIN dbo.Departments d ON d.DepartmentId=t.DepartmentId LEFT JOIN dbo.TestResults tr ON tr.OrderItemId=oi.OrderItemId WHERE o.OrderId=@id AND oi.Status IN ('Entered','Verified') ORDER BY t.TestName";
        await using var cmd = new SqlCommand(sql, c);
        cmd.Parameters.AddWithValue("@id", orderId);
        await using var r = await cmd.ExecuteReaderAsync();
        ReportData? data = null;

        while (await r.ReadAsync())
        {
            if (data == null)
                data = new ReportData(
                    r.GetString(0),
                    r.IsDBNull(1) ? null : r.GetString(1),
                    r.GetString(2),
                    r.GetString(3),
                    r.GetString(4),
                    r.IsDBNull(5) ? null : r.GetString(5),
                    r.GetDateTime(6),
                    r.IsDBNull(7) ? null : r.GetString(7),
                    r.IsDBNull(8) ? null : r.GetDateTime(8),
                    r.IsDBNull(9) ? null : r.GetDateTime(9),
                    new());

            data.Results.Add(new ReportResult(
                r.GetString(10),
                r.GetString(11),
                r.GetString(12),
                r.IsDBNull(13) ? null : r.GetString(13),
                r.IsDBNull(14) ? null : r.GetString(14),
                r.IsDBNull(15) ? null : r.GetString(15),
                r.IsDBNull(16) ? null : r.GetString(16)));
        }

        return data;
    }

    public async Task VerifyAsync(int orderId, string verifier)
    {
        if (!PermissionService.Can(CurrentUserContext.Role, "Reports"))
            throw new UnauthorizedAccessException("Your role is not allowed to verify reports.");

        if (string.IsNullOrWhiteSpace(verifier))
            throw new InvalidOperationException("Verifier identity is required.");

        await using var c = Database.CreateConnection();
        await c.OpenAsync();
        await using var tx = await c.BeginTransactionAsync();

        try
        {
            const string countSql = "SELECT COUNT(1) FROM dbo.TestOrderItems WHERE OrderId=@id AND Status='Entered'";
            await using var count = new SqlCommand(countSql, c, (SqlTransaction)tx);
            count.Parameters.AddWithValue("@id", orderId);
            var enteredCount = Convert.ToInt32(await count.ExecuteScalarAsync());

            if (enteredCount == 0)
                throw new InvalidOperationException("No entered results are waiting for verification.");

            const string sql = """
                UPDATE dbo.TestOrderItems
                SET Status='Verified', VerifiedBy=@v
                WHERE OrderId=@id AND Status='Entered';

                UPDATE dbo.TestResults
                SET ResultStatus='Verified', VerifiedBy=@v, VerifiedAt=SYSDATETIME()
                WHERE OrderItemId IN
                (
                    SELECT OrderItemId FROM dbo.TestOrderItems
                    WHERE OrderId=@id AND Status='Verified'
                );

                UPDATE dbo.TestOrders
                SET Status='Verified', VerifiedBy=@v, VerifiedAt=SYSDATETIME(), ReportIssuedAt=SYSDATETIME()
                WHERE OrderId=@id;
                """;

            await using var cmd = new SqlCommand(sql, c, (SqlTransaction)tx);
            cmd.Parameters.AddWithValue("@v", verifier.Trim());
            cmd.Parameters.AddWithValue("@id", orderId);
            await cmd.ExecuteNonQueryAsync();

            await tx.CommitAsync();

            await new AuditService().WriteAsync(
                verifier.Trim(),
                "VERIFY",
                "TestOrder",
                orderId,
                "Report verified");
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }

    public async Task<string> EnsureReportNumberAsync(int orderId)
    {
        await using var c = Database.CreateConnection();
        await c.OpenAsync();
        await using var tx = await c.BeginTransactionAsync();

        const string sql = """
                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
                DECLARE @existing NVARCHAR(40);
                SELECT @existing=ReportNumber FROM dbo.TestOrders WHERE OrderId=@id;

                IF @existing IS NOT NULL
                    SELECT @existing;
                ELSE
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM dbo.TestOrders WHERE OrderId=@id)
                        THROW 50001, 'Test order was not found.', 1;

                    DECLARE @d DATE=CAST(SYSDATETIME() AS DATE);

                    IF NOT EXISTS
                    (
                        SELECT 1
                        FROM dbo.ReportNumberSequence WITH (UPDLOCK,HOLDLOCK)
                        WHERE SequenceDate=@d
                    )
                    BEGIN
                        INSERT dbo.ReportNumberSequence(SequenceDate,LastNumber)
                        VALUES(@d,0);
                    END

                    UPDATE dbo.ReportNumberSequence
                    SET LastNumber=LastNumber+1
                    WHERE SequenceDate=@d;

                    DECLARE @n INT=(SELECT LastNumber FROM dbo.ReportNumberSequence WHERE SequenceDate=@d);
                    DECLARE @rn NVARCHAR(40)=CONCAT('RPT-',CONVERT(char(8),@d,112),'-',FORMAT(@n,'0000'));

                    UPDATE dbo.TestOrders
                    SET ReportNumber=@rn
                    WHERE OrderId=@id;

                    SELECT @rn;
                END
                """;
        await using var cmd = new SqlCommand(sql, c, (SqlTransaction)tx);
        cmd.Parameters.AddWithValue("@id", orderId);
        var result = await cmd.ExecuteScalarAsync();
        await tx.CommitAsync();
        return Convert.ToString(result) ?? "";
    }
}

public record ReportOrder(int OrderId, string OrderNumber, string? ReportNumber, string MRNumber, string PatientName, string Gender, DateTime OrderDate, string Status);
public sealed record ReportData(string OrderNumber, string? ReportNumber, string MRNumber, string PatientName, string Gender, string? Phone, DateTime OrderDate, string? VerifiedBy, DateTime? VerifiedAt, DateTime? ReportIssuedAt, List<ReportResult> Results);
public record ReportResult(string Code, string TestName, string Department, string? Unit, string? ReferenceRange, string? ResultValue, string? Comment);