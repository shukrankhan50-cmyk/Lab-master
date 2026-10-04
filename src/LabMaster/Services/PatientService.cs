using LabMaster.Data;
using LabMaster.Models;
using Microsoft.Data.SqlClient;

namespace LabMaster.Services;

public sealed class PatientService
{
    public async Task<string> GetNextMrNumberAsync()
    {
        await using var connection = Database.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand("SELECT ISNULL(MAX(PatientId), 0) + 1 FROM dbo.Patients;", connection);
        var nextId = Convert.ToInt32(await command.ExecuteScalarAsync());
        return $"MR-{nextId:000000}";
    }

    public async Task<int> SaveAsync(Patient patient)
    {
        await using var connection = Database.CreateConnection();
        await connection.OpenAsync();

        const string sql = """
            INSERT INTO dbo.Patients
            (MRNumber, PatientName, FatherName, Gender, DateOfBirth, AgeYears, AgeMonths, AgeDays, Phone, Address, ReferringDoctor)
            OUTPUT INSERTED.PatientId
            VALUES
            (@MRNumber, @PatientName, @FatherName, @Gender, @DateOfBirth, @AgeYears, @AgeMonths, @AgeDays, @Phone, @Address, @ReferringDoctor);
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@MRNumber", patient.MRNumber);
        command.Parameters.AddWithValue("@PatientName", patient.PatientName);
        command.Parameters.AddWithValue("@FatherName", (object?)patient.FatherName ?? DBNull.Value);
        command.Parameters.AddWithValue("@Gender", patient.Gender);
        command.Parameters.AddWithValue("@DateOfBirth", (object?)patient.DateOfBirth ?? DBNull.Value);
        command.Parameters.AddWithValue("@AgeYears", (object?)patient.AgeYears ?? DBNull.Value);
        command.Parameters.AddWithValue("@AgeMonths", (object?)patient.AgeMonths ?? DBNull.Value);
        command.Parameters.AddWithValue("@AgeDays", (object?)patient.AgeDays ?? DBNull.Value);
        command.Parameters.AddWithValue("@Phone", (object?)patient.Phone ?? DBNull.Value);
        command.Parameters.AddWithValue("@Address", (object?)patient.Address ?? DBNull.Value);
        command.Parameters.AddWithValue("@ReferringDoctor", (object?)patient.ReferringDoctor ?? DBNull.Value);

        var id = Convert.ToInt32(await command.ExecuteScalarAsync());
        await new AuditService().WriteAsync(CurrentUserContext.UserName, "CREATE", "Patient", id, $"Registered {patient.MRNumber}");
        return id;
    }
}