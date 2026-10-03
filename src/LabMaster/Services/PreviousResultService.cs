using LabMaster.Data;
using Microsoft.Data.SqlClient;
namespace LabMaster.Services;
public sealed class PreviousResultService
{
 public async Task<List<PreviousResult>> GetAsync(string mrNumber,string testCode)
 {
  await using var c=Database.CreateConnection();await c.OpenAsync();
  const string sql="SELECT TOP 20 o.OrderNumber,o.OrderDate,oi.ResultValue,tr.ResultComment FROM dbo.TestOrders o JOIN dbo.Patients p ON p.PatientId=o.PatientId JOIN dbo.TestOrderItems oi ON oi.OrderId=o.OrderId JOIN dbo.Tests t ON t.TestId=oi.TestId LEFT JOIN dbo.TestResults tr ON tr.OrderItemId=oi.OrderItemId WHERE p.MRNumber=@mr AND t.TestCode=@code AND oi.ResultValue IS NOT NULL ORDER BY o.OrderDate DESC";
  await using var cmd=new SqlCommand(sql,c);cmd.Parameters.AddWithValue("@mr",mrNumber);cmd.Parameters.AddWithValue("@code",testCode);
  await using var r=await cmd.ExecuteReaderAsync();var list=new List<PreviousResult>();
  while(await r.ReadAsync())list.Add(new PreviousResult(r.GetString(0),r.GetDateTime(1),r.IsDBNull(2)?null:r.GetString(2),r.IsDBNull(3)?null:r.GetString(3)));
  return list;
 }
}
public record PreviousResult(string OrderNumber,DateTime Date,string? Result,string? Comment);