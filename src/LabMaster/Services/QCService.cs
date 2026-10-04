using LabMaster.Data;
using Microsoft.Data.SqlClient;

namespace LabMaster.Services;

public sealed class QCService
{
    public async Task<List<QCRow>> GetAsync()
    {
        await using var c = Database.CreateConnection();
        await c.OpenAsync();
        const string s = "SELECT q.QCId,q.MaterialName,q.LotNumber,t.TestName,q.TargetMean,q.SD,q.ExpiryDate FROM dbo.QCMaterials q LEFT JOIN dbo.Tests t ON t.TestId=q.TestId ORDER BY q.MaterialName";
        await using var cmd = new SqlCommand(s, c);
        await using var r = await cmd.ExecuteReaderAsync();
        var x = new List<QCRow>();
        while (await r.ReadAsync())
            x.Add(new QCRow(r.GetInt32(0), r.GetString(1), r.IsDBNull(2) ? null : r.GetString(2), r.IsDBNull(3) ? null : r.GetString(3), r.IsDBNull(4) ? null : r.GetDecimal(4), r.IsDBNull(5) ? null : r.GetDecimal(5), r.IsDBNull(6) ? null : r.GetDateTime(6)));
        return x;
    }

    public async Task AddAsync(string material, string? lot, int? testId, decimal mean, decimal sd, DateTime? expiry)
    {
        if (string.IsNullOrWhiteSpace(material))
            throw new InvalidOperationException("QC material name is required.");
        if (sd <= 0)
            throw new InvalidOperationException("QC standard deviation must be greater than zero.");

        await using var c = Database.CreateConnection();
        await c.OpenAsync();
        const string s = "INSERT dbo.QCMaterials(TestId,MaterialName,LotNumber,TargetMean,SD,ExpiryDate) VALUES(@t,@m,@l,@mean,@sd,@e)";
        await using var cmd = new SqlCommand(s, c);
        cmd.Parameters.AddWithValue("@t", (object?)testId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@m", material.Trim());
        cmd.Parameters.AddWithValue("@l", (object?)lot ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@mean", mean);
        cmd.Parameters.AddWithValue("@sd", sd);
        cmd.Parameters.AddWithValue("@e", (object?)expiry ?? DBNull.Value);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<string> EvaluateAsync(int qcId, decimal value)
    {
        await using var c = Database.CreateConnection();
        await c.OpenAsync();
        const string s = "SELECT TargetMean,SD FROM dbo.QCMaterials WHERE QCId=@id AND IsActive=1";
        await using var cmd = new SqlCommand(s, c);
        cmd.Parameters.AddWithValue("@id", qcId);
        await using var r = await cmd.ExecuteReaderAsync();
        if (!await r.ReadAsync())
            throw new InvalidOperationException("QC material not found.");

        if (r.IsDBNull(0) || r.IsDBNull(1))
            return "NO RANGE";

        var mean = r.GetDecimal(0);
        var sd = r.GetDecimal(1);
        if (sd <= 0)
            return "NO RANGE";

        var z = Math.Abs((double)(value - mean) / (double)sd);
        return z <= 2 ? "ACCEPT" : z <= 3 ? "WARNING" : "REJECT";
    }

    public async Task SaveResultAsync(int qcId, decimal value, string? comment)
    {
        await using var c = Database.CreateConnection();
        await c.OpenAsync();
        const string s = "INSERT dbo.QCResults(QCId,ResultValue,EnteredBy,Comment) VALUES(@id,@v,@u,@c)";
        await using var cmd = new SqlCommand(s, c);
        cmd.Parameters.AddWithValue("@id", qcId);
        cmd.Parameters.AddWithValue("@v", value);
        cmd.Parameters.AddWithValue("@u", CurrentUserContext.UserName);
        cmd.Parameters.AddWithValue("@c", (object?)comment ?? DBNull.Value);
        await cmd.ExecuteNonQueryAsync();

        await new AuditService().WriteAsync(
            CurrentUserContext.UserName,
            "QC_RESULT",
            "QCMaterial",
            qcId,
            $"QC result {value:0.####}");
    }

    public async Task<List<QCResultRow>> GetResultsAsync(int qcId)
    {
        await using var c = Database.CreateConnection();
        await c.OpenAsync();
        const string s = "SELECT TOP 50 QCResultId,ResultValue,RunDate,EnteredBy,Comment FROM dbo.QCResults WHERE QCId=@id ORDER BY QCResultId DESC";
        await using var cmd = new SqlCommand(s, c);
        cmd.Parameters.AddWithValue("@id", qcId);
        await using var r = await cmd.ExecuteReaderAsync();
        var x = new List<QCResultRow>();
        while (await r.ReadAsync())
            x.Add(new QCResultRow(r.GetInt32(0), r.IsDBNull(1) ? null : r.GetDecimal(1), r.GetDateTime(2), r.IsDBNull(3) ? null : r.GetString(3), r.IsDBNull(4) ? null : r.GetString(4)));
        return x;
    }
}

public record QCRow(int Id, string Material, string? Lot, string? Test, decimal? Mean, decimal? SD, DateTime? Expiry);
public record QCResultRow(int Id, decimal? Value, DateTime RunDate, string? EnteredBy, string? Comment);
