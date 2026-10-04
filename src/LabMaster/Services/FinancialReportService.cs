using LabMaster.Data;
using Microsoft.Data.SqlClient;

namespace LabMaster.Services;

public sealed class FinancialReportService
{
    public async Task<FinancialSummary> GetAsync(DateTime from, DateTime to)
    {
        await using var c = Database.CreateConnection();
        await c.OpenAsync();

        const string s = "SELECT ISNULL((SELECT SUM(Amount) FROM dbo.Payments WHERE PaidAt>=@f AND PaidAt<@t),0),ISNULL((SELECT SUM(Amount) FROM dbo.Expenses WHERE ExpenseDate>=@f AND ExpenseDate<@t),0)";
        await using var cmd = new SqlCommand(s, c);
        cmd.Parameters.AddWithValue("@f", from.Date);
        cmd.Parameters.AddWithValue("@t", to.Date.AddDays(1));
        await using var r = await cmd.ExecuteReaderAsync();
        await r.ReadAsync();
        var income = r.GetDecimal(0);
        var expense = r.GetDecimal(1);
        await r.CloseAsync();

        const string detail = "SELECT ISNULL(SUM(TotalAmount),0),COUNT(1) FROM dbo.TestOrders WHERE OrderDate>=@f AND OrderDate<@t";
        await using var detailCmd = new SqlCommand(detail, c);
        detailCmd.Parameters.AddWithValue("@f", from.Date);
        detailCmd.Parameters.AddWithValue("@t", to.Date.AddDays(1));
        await using var d = await detailCmd.ExecuteReaderAsync();
        await d.ReadAsync();
        var billed = d.GetDecimal(0);
        var orders = d.GetInt32(1);

        return new FinancialSummary(income, expense, income - expense, billed, orders);
    }
}

public record FinancialSummary(decimal Collection, decimal Expenses, decimal Net, decimal Billed, int Orders);