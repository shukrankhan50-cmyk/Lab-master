using LabMaster.Data;
using Microsoft.Data.SqlClient;
namespace LabMaster.Services;
public sealed class ResultEntryService
{
 public async Task<List<PendingResult>> GetPendingAsync()
 {
  await using var c=Database.CreateConnection();await c.OpenAsync();
  const string sql="SELECT oi.OrderItemId,o.OrderNumber,p.MRNumber,p.PatientName,t.TestCode,t.TestName,d.DepartmentName,oi.ResultValue,t.Unit,t.ReferenceRange,t.CriticalLow,t.CriticalHigh FROM dbo.TestOrderItems oi JOIN dbo.TestOrders o ON o.OrderId=oi.OrderId JOIN dbo.Patients p ON p.PatientId=o.PatientId JOIN dbo.Tests t ON t.TestId=oi.TestId JOIN dbo.Departments d ON d.DepartmentId=t.DepartmentId WHERE oi.Status IN ('Pending','Entered') ORDER BY o.OrderId DESC,oi.OrderItemId";
  await using var cmd=new SqlCommand(sql,c);await using var r=await cmd.ExecuteReaderAsync();var list=new List<PendingResult>();
  while(await r.ReadAsync()) list.Add(new PendingResult(r.GetInt32(0),r.GetString(1),r.GetString(2),r.GetString(3),r.GetString(4),r.GetString(5),r.GetString(6),r.IsDBNull(7)?null:r.GetString(7),r.IsDBNull(8)?null:r.GetString(8),r.IsDBNull(9)?null:r.GetString(9),r.IsDBNull(10)?null:r.GetDecimal(10),r.IsDBNull(11)?null:r.GetDecimal(11)));
  return list;
 }
 public async Task SaveResultAsync(int itemId,string value,string? comment)
 {
  await using var c=Database.CreateConnection();await c.OpenAsync();await using var tx=await c.BeginTransactionAsync();
  const string sql="IF EXISTS(SELECT 1 FROM dbo.TestResults WHERE OrderItemId=@id) UPDATE dbo.TestResults SET ResultValue=@v,ResultComment=@c,ResultStatus='Entered',EnteredAt=SYSDATETIME() WHERE OrderItemId=@id ELSE INSERT dbo.TestResults(OrderItemId,ResultValue,ResultComment,ResultStatus,EnteredAt) VALUES(@id,@v,@c,'Entered',SYSDATETIME()); UPDATE dbo.TestOrderItems SET ResultValue=@v,Status='Entered' WHERE OrderItemId=@id;";
  await using var cmd=new SqlCommand(sql,c,(SqlTransaction)tx);cmd.Parameters.AddWithValue("@id",itemId);cmd.Parameters.AddWithValue("@v",value);cmd.Parameters.AddWithValue("@c",(object?)comment??DBNull.Value);await cmd.ExecuteNonQueryAsync();await tx.CommitAsync();
  await new AuditService().WriteAsync(CurrentUserContext.UserName, "RESULT_ENTRY", "TestOrderItem", itemId, $"Result entered: {value}");
 }
}
public record PendingResult(int OrderItemId,string OrderNumber,string MRNumber,string PatientName,string TestCode,string TestName,string DepartmentName,string? ResultValue,string? Unit,string? ReferenceRange,decimal? CriticalLow,decimal? CriticalHigh);