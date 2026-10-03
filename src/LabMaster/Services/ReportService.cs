using LabMaster.Data;
using Microsoft.Data.SqlClient;
namespace LabMaster.Services;
public sealed class ReportService
{
 public async Task<List<ReportOrder>> GetEnteredOrdersAsync()
 {
  await using var c=Database.CreateConnection(); await c.OpenAsync();
  const string sql="SELECT DISTINCT o.OrderId,o.OrderNumber,p.MRNumber,p.PatientName,p.Gender,o.OrderDate FROM dbo.TestOrders o JOIN dbo.Patients p ON p.PatientId=o.PatientId JOIN dbo.TestOrderItems oi ON oi.OrderId=o.OrderId WHERE oi.Status='Entered' ORDER BY o.OrderId DESC";
  await using var cmd=new SqlCommand(sql,c); await using var r=await cmd.ExecuteReaderAsync(); var list=new List<ReportOrder>();
  while(await r.ReadAsync()) list.Add(new ReportOrder(r.GetInt32(0),r.GetString(1),r.GetString(2),r.GetString(3),r.GetString(4),r.GetDateTime(5)));
  return list;
 }
 public async Task<ReportData?> GetReportAsync(int orderId)
 {
  await using var c=Database.CreateConnection(); await c.OpenAsync();
  const string sql="SELECT o.OrderNumber,p.MRNumber,p.PatientName,p.Gender,p.Phone,o.OrderDate,t.TestCode,t.TestName,d.DepartmentName,t.Unit,t.ReferenceRange,oi.ResultValue,tr.ResultComment FROM dbo.TestOrders o JOIN dbo.Patients p ON p.PatientId=o.PatientId JOIN dbo.TestOrderItems oi ON oi.OrderId=o.OrderId JOIN dbo.Tests t ON t.TestId=oi.TestId JOIN dbo.Departments d ON d.DepartmentId=t.DepartmentId LEFT JOIN dbo.TestResults tr ON tr.OrderItemId=oi.OrderItemId WHERE o.OrderId=@id AND oi.Status='Entered' ORDER BY t.TestName";
  await using var cmd=new SqlCommand(sql,c);cmd.Parameters.AddWithValue("@id",orderId);await using var r=await cmd.ExecuteReaderAsync();
  ReportData? data=null;
  while(await r.ReadAsync()){if(data==null)data=new ReportData(r.GetString(0),r.GetString(1),r.GetString(2),r.GetString(3),r.IsDBNull(4)?null:r.GetString(4),r.GetDateTime(5),new());
   data.Results.Add(new ReportResult(r.GetString(6),r.GetString(7),r.GetString(8),r.IsDBNull(9)?null:r.GetString(9),r.IsDBNull(10)?null:r.GetString(10),r.IsDBNull(11)?null:r.GetString(11),r.IsDBNull(12)?null:r.GetString(12)));}
  return data;
 }
 public async Task VerifyAsync(int orderId,string verifier)
 {
  await using var c=Database.CreateConnection();await c.OpenAsync();
  const string sql="UPDATE dbo.TestOrderItems SET Status='Verified',VerifiedBy=@v WHERE OrderId=@id AND Status='Entered'; UPDATE dbo.TestResults SET ResultStatus='Verified',VerifiedBy=@v,VerifiedAt=SYSDATETIME() WHERE OrderItemId IN (SELECT OrderItemId FROM dbo.TestOrderItems WHERE OrderId=@id); UPDATE dbo.TestOrders SET Status='Verified' WHERE OrderId=@id;";
  await using var cmd=new SqlCommand(sql,c);cmd.Parameters.AddWithValue("@v",verifier);cmd.Parameters.AddWithValue("@id",orderId);await cmd.ExecuteNonQueryAsync();
 }
}
public record ReportOrder(int OrderId,string OrderNumber,string MRNumber,string PatientName,string Gender,DateTime OrderDate);
public sealed record ReportData(string OrderNumber,string MRNumber,string PatientName,string Gender,string? Phone,DateTime OrderDate,List<ReportResult> Results);
public record ReportResult(string Code,string TestName,string Department,string? Unit,string? ReferenceRange,string? ResultValue,string? Comment);