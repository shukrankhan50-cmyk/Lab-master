using LabMaster.Data;
using Microsoft.Data.SqlClient;

namespace LabMaster.Services;

public sealed class AnalyzerService
{
    public async Task<List<AnalyzerRow>> GetAsync()
    {
        await using var c = Database.CreateConnection();
        await c.OpenAsync();

        const string s = """
            SELECT a.AnalyzerId,a.AnalyzerCode,a.AnalyzerName,d.DepartmentName,
                   a.Manufacturer,a.Model,a.SerialNumber,a.ConnectionType,a.IsActive,
                   a.LastCalibrationDate,a.NextCalibrationDate,a.Notes
            FROM dbo.Analyzers a
            LEFT JOIN dbo.Departments d ON d.DepartmentId=a.DepartmentId
            ORDER BY a.AnalyzerName
            """;

        await using var cmd = new SqlCommand(s, c);
        await using var r = await cmd.ExecuteReaderAsync();

        var x = new List<AnalyzerRow>();
        while (await r.ReadAsync())
            x.Add(new AnalyzerRow(
                r.GetInt32(0), r.GetString(1), r.GetString(2),
                r.IsDBNull(3) ? null : r.GetString(3),
                r.IsDBNull(4) ? null : r.GetString(4),
                r.IsDBNull(5) ? null : r.GetString(5),
                r.IsDBNull(6) ? null : r.GetString(6),
                r.IsDBNull(7) ? null : r.GetString(7),
                r.GetBoolean(8),
                r.IsDBNull(9) ? null : r.GetDateTime(9),
                r.IsDBNull(10) ? null : r.GetDateTime(10),
                r.IsDBNull(11) ? null : r.GetString(11)));

        return x;
    }

    public async Task AddAsync(
        string code, string name, int? departmentId, string? manufacturer,
        string? model, string? serial, string? connection,
        DateTime? lastCalibration, DateTime? nextCalibration, string? notes)
    {
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(name))
            throw new InvalidOperationException("Analyzer code and name are required.");

        if (nextCalibration.HasValue && lastCalibration.HasValue &&
            nextCalibration.Value.Date < lastCalibration.Value.Date)
            throw new InvalidOperationException("Next calibration date cannot be earlier than the last calibration date.");

        await using var c = Database.CreateConnection();
        await c.OpenAsync();

        const string exists = "SELECT COUNT(1) FROM dbo.Analyzers WHERE AnalyzerCode=@c";
        await using (var check = new SqlCommand(exists, c))
        {
            check.Parameters.AddWithValue("@c", code.Trim());
            if (Convert.ToInt32(await check.ExecuteScalarAsync()) > 0)
                throw new InvalidOperationException("Analyzer code already exists.");
        }

        const string s = """
            INSERT dbo.Analyzers
            (AnalyzerCode,AnalyzerName,DepartmentId,Manufacturer,Model,SerialNumber,
             ConnectionType,LastCalibrationDate,NextCalibrationDate,Notes)
            VALUES
            (@c,@n,@d,@m,@mo,@sn,@ct,@lc,@nc,@no)
            """;

        await using var cmd = new SqlCommand(s, c);
        cmd.Parameters.AddWithValue("@c", code.Trim());
        cmd.Parameters.AddWithValue("@n", name.Trim());
        cmd.Parameters.AddWithValue("@d", (object?)departmentId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@m", (object?)manufacturer ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@mo", (object?)model ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@sn", (object?)serial ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ct", (object?)connection ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@lc", (object?)lastCalibration ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@nc", (object?)nextCalibration ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@no", (object?)notes ?? DBNull.Value);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task SetActiveAsync(int id, bool active)
    {
        await using var c = Database.CreateConnection();
        await c.OpenAsync();
        await using var cmd = new SqlCommand(
            "UPDATE dbo.Analyzers SET IsActive=@a WHERE AnalyzerId=@id", c);
        cmd.Parameters.AddWithValue("@a", active);
        cmd.Parameters.AddWithValue("@id", id);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<List<AnalyzerMaintenanceRow>> GetMaintenanceAsync(int analyzerId)
    {
        await using var c = Database.CreateConnection();
        await c.OpenAsync();

        const string s = """
            SELECT MaintenanceId,MaintenanceDate,MaintenanceType,Description,
                   PerformedBy,NextDueDate,Cost,CertificateNumber,Result,Notes
            FROM dbo.AnalyzerMaintenance
            WHERE AnalyzerId=@id
            ORDER BY MaintenanceDate DESC, MaintenanceId DESC
            """;

        await using var cmd = new SqlCommand(s, c);
        cmd.Parameters.AddWithValue("@id", analyzerId);
        await using var r = await cmd.ExecuteReaderAsync();

        var list = new List<AnalyzerMaintenanceRow>();
        while (await r.ReadAsync())
            list.Add(new AnalyzerMaintenanceRow(
                r.GetInt32(0), r.GetDateTime(1), r.GetString(2),
                r.IsDBNull(3) ? null : r.GetString(3),
                r.IsDBNull(4) ? null : r.GetString(4),
                r.IsDBNull(5) ? null : r.GetDateTime(5),
                r.IsDBNull(6) ? null : r.GetDecimal(6),
                r.IsDBNull(7) ? null : r.GetString(7),
                r.IsDBNull(8) ? null : r.GetString(8),
                r.IsDBNull(9) ? null : r.GetString(9)));

        return list;
    }

    public async Task AddMaintenanceAsync(
        int analyzerId, DateTime date, string type, string? description,
        string? performedBy, DateTime? nextDueDate, decimal? cost,
        string? certificateNumber, string? result, string? notes)
    {
        if (string.IsNullOrWhiteSpace(type))
            throw new InvalidOperationException("Maintenance type is required.");

        if (nextDueDate.HasValue && nextDueDate.Value.Date < date.Date)
            throw new InvalidOperationException("Next due date cannot be earlier than the maintenance date.");

        await using var c = Database.CreateConnection();
        await c.OpenAsync();

        const string s = """
            INSERT dbo.AnalyzerMaintenance
            (AnalyzerId,MaintenanceDate,MaintenanceType,Description,PerformedBy,
             NextDueDate,Cost,CertificateNumber,Result,Notes)
            VALUES
            (@a,@d,@t,@desc,@by,@next,@cost,@cert,@result,@notes);

            UPDATE dbo.Analyzers
            SET LastCalibrationDate =
                    CASE WHEN @t IN ('Calibration','Calibration/Verification')
                         THEN @d ELSE LastCalibrationDate END,
                NextCalibrationDate =
                    CASE WHEN @t IN ('Calibration','Calibration/Verification')
                         THEN @next ELSE NextCalibrationDate END
            WHERE AnalyzerId=@a;
            """;

        await using var cmd = new SqlCommand(s, c);
        cmd.Parameters.AddWithValue("@a", analyzerId);
        cmd.Parameters.AddWithValue("@d", date.Date);
        cmd.Parameters.AddWithValue("@t", type.Trim());
        cmd.Parameters.AddWithValue("@desc", (object?)description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@by", string.IsNullOrWhiteSpace(performedBy)
            ? CurrentUserContext.UserName : performedBy.Trim());
        cmd.Parameters.AddWithValue("@next", (object?)nextDueDate?.Date ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@cost", (object?)cost ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@cert", (object?)certificateNumber ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@result", (object?)result ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@notes", (object?)notes ?? DBNull.Value);
        await cmd.ExecuteNonQueryAsync();

        await new AuditService().WriteAsync(
            CurrentUserContext.UserName,
            "ANALYZER_MAINTENANCE",
            "Analyzer",
            analyzerId,
            $"Added {type.Trim()} record");
    }
}

public record AnalyzerRow(
    int Id, string Code, string Name, string? Department, string? Manufacturer,
    string? Model, string? SerialNumber, string? ConnectionType, bool Active,
    DateTime? LastCalibration, DateTime? NextCalibration, string? Notes);

public record AnalyzerMaintenanceRow(
    int Id, DateTime Date, string Type, string? Description, string? PerformedBy,
    DateTime? NextDueDate, decimal? Cost, string? CertificateNumber,
    string? Result, string? Notes);
