using LabMaster.Data;
using Microsoft.Data.SqlClient;

namespace LabMaster.Services;

public sealed class TestOrderService
{
    public async Task<List<TestOption>> GetTestsAsync(string term = "")
    {
        await using var c = Database.CreateConnection();
        await c.OpenAsync();

        const string sql = "SELECT t.TestId,t.TestCode,t.TestName,d.DepartmentName,t.Price FROM dbo.Tests t INNER JOIN dbo.Departments d ON d.DepartmentId=t.DepartmentId WHERE t.IsActive=1 AND (t.TestCode LIKE @q OR t.TestName LIKE @q) ORDER BY t.TestName";
        await using var cmd = new SqlCommand(sql, c);
        cmd.Parameters.AddWithValue("@q", "%" + term.Trim() + "%");

        var list = new List<TestOption>();
        await using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync())
            list.Add(new TestOption(r.GetInt32(0), r.GetString(1), r.GetString(2), r.GetString(3), r.GetDecimal(4)));

        return list;
    }

    public async Task<string> CreateOrderAsync(int patientId, IEnumerable<int> testIds)
    {
        var ids = testIds.Distinct().ToList();
        if (ids.Count == 0)
            throw new InvalidOperationException("Select at least one test.");

        await using var c = Database.CreateConnection();
        await c.OpenAsync();
        await using var tx = await c.BeginTransactionAsync();

        try
        {
            const string nextOrderSql = "SELECT ISNULL(MAX(OrderId), 0) + 1 FROM dbo.TestOrders WITH(TABLOCKX);";
            await using var nextOrderCommand = new SqlCommand(nextOrderSql, c, (SqlTransaction)tx);
            var nextId = Convert.ToInt32(await nextOrderCommand.ExecuteScalarAsync());
            var orderNo = $"ORD-{nextId:000000}";

            const string patientCheckSql = "SELECT COUNT(1) FROM dbo.Patients WHERE PatientId=@patient";
            await using var patientCheck = new SqlCommand(patientCheckSql, c, (SqlTransaction)tx);
            patientCheck.Parameters.AddWithValue("@patient", patientId);
            if (Convert.ToInt32(await patientCheck.ExecuteScalarAsync()) == 0)
                throw new InvalidOperationException("Patient not found.");

            decimal total = 0;
            foreach (var id in ids)
            {
                await using var p = new SqlCommand(
                    "SELECT Price FROM dbo.Tests WHERE TestId=@id AND IsActive=1",
                    c,
                    (SqlTransaction)tx);
                p.Parameters.AddWithValue("@id", id);
                var v = await p.ExecuteScalarAsync();

                if (v == null)
                    throw new InvalidOperationException("A selected test is unavailable.");

                total += Convert.ToDecimal(v);
            }

            int orderId;
            await using (var cmd = new SqlCommand(
                "INSERT dbo.TestOrders(OrderNumber,PatientId,TotalAmount) OUTPUT INSERTED.OrderId VALUES(@no,@patient,@total)",
                c,
                (SqlTransaction)tx))
            {
                cmd.Parameters.AddWithValue("@no", orderNo);
                cmd.Parameters.AddWithValue("@patient", patientId);
                cmd.Parameters.AddWithValue("@total", total);
                orderId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
            }

            foreach (var id in ids)
            {
                await using var cmd = new SqlCommand(
                    "INSERT dbo.TestOrderItems(OrderId,TestId) VALUES(@o,@t)",
                    c,
                    (SqlTransaction)tx);
                cmd.Parameters.AddWithValue("@o", orderId);
                cmd.Parameters.AddWithValue("@t", id);
                await cmd.ExecuteNonQueryAsync();
            }

            await tx.CommitAsync();

            await new AuditService().WriteAsync(
                CurrentUserContext.UserName,
                "CREATE",
                "TestOrder",
                orderId,
                $"Created {orderNo} with {ids.Count} test(s)");

            return orderNo;
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }
}

public record TestOption(int TestId, string TestCode, string TestName, string DepartmentName, decimal Price);