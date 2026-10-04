using LabMaster.Data;
using Microsoft.Data.SqlClient;
namespace LabMaster.Services;
public sealed class FinancialReportService
{
 public async Task<FinancialSummary> GetAsync(DateTime from,DateTime to)
 {
  await using var c=Database.CreateConnection();await c.OpenAsync();
  const string s="SELECT ISNULL((SELECT SUM(Amount) FROM dbo.Payments WHERE PaidAt>=@f AND PaidAt<@t),0),ISNULL((SELECT SUM(Amount) FROM dbo.Expenses WHERE ExpenseDate>=@f AND ExpenseDate<@t),0)";
  await using var cmd=new SqlCommand(s,c);cmd.Parameters.AddWithValue("@f",from.Date);cmd.Parameters.AddWithValue("@t",to.Date.AddDays(1));
  await using var r=await cmd.ExecuteReaderAsync();await r.ReadAsync();
  var income=r.GetDecimal(0);var expense=r.GetDecimal(1);return new FinancialSummary(income,expense,income-expense);
 }
}
public record FinancialSummary(decimal Collection,decimal Expenses,decimal Net);