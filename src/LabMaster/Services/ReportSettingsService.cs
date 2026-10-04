using LabMaster.Data;
using Microsoft.Data.SqlClient;

namespace LabMaster.Services;

public sealed class ReportSettingsService
{
    public async Task<ReportSettings> GetAsync()
    {
        await using var c = Database.CreateConnection();
        await c.OpenAsync();

        const string sql = """
            SELECT TOP 1 LaboratoryName, Address, Phone, ReportFooter
            FROM dbo.ReportSettings
            ORDER BY SettingId DESC;
            """;

        await using var cmd = new SqlCommand(sql, c);
        await using var r = await cmd.ExecuteReaderAsync();

        if (await r.ReadAsync())
        {
            return new ReportSettings(
                r.GetString(0),
                r.IsDBNull(1) ? null : r.GetString(1),
                r.IsDBNull(2) ? null : r.GetString(2),
                r.IsDBNull(3) ? null : r.GetString(3));
        }

        return new ReportSettings(
            "Lab Master Laboratory",
            "Laboratory Information System",
            null,
            "Computer generated laboratory report");
    }
}

public record ReportSettings(
    string LaboratoryName,
    string? Address,
    string? Phone,
    string? ReportFooter);
