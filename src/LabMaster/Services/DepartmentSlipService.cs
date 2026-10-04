using LabMaster.Data;
using Microsoft.Data.SqlClient;
namespace LabMaster.Services;
public sealed class DepartmentSlipService
{
 public async Task<List<SlipOrder>> GetOrdersAsync(){await using var c=Database.CreateConnection();await c.OpenAsync();const string s="SELECT TOP 200 o.OrderId,o.OrderNumber,p.MRNumber,p.PatientName,o.OrderDate FROM dbo.TestOrders o JOIN dbo.Patients p ON p.PatientId=o.PatientId WHERE EXISTS(SELECT 1 FROM dbo.TestOrderItems oi WHERE oi.OrderId=o.OrderId) ORDER BY o.OrderId DESC";await using var cmd=new SqlCommand(s,c);await using var r=await cmd.ExecuteReaderAsync();var x=new List<SlipOrder>();while(await r.ReadAsync())x.Add(new SlipOrder(r.GetInt32(0),r.GetString(1),r.GetString(2),r.GetString(3),r.GetDateTime(4)));return x;}
 public async Task<List<SlipItem>> GetItemsAsync(int orderId){await using var c=Database.CreateConnection();await c.OpenAsync();const string s="SELECT d.DepartmentName,t.TestCode,t.TestName,oi.Status FROM dbo.TestOrderItems oi JOIN dbo.Tests t ON t.TestId=oi.TestId JOIN dbo.Departments d ON d.DepartmentId=t.DepartmentId WHERE oi.OrderId=@id ORDER BY d.DepartmentName,t.TestName";await using var cmd=new SqlCommand(s,c);cmd.Parameters.AddWithValue("@id",orderId);await using var r=await cmd.ExecuteReaderAsync();var x=new List<SlipItem>();while(await r.ReadAsync())x.Add(new SlipItem(r.GetString(0),r.GetString(1),r.GetString(2),r.GetString(3)));return x;}
}
public record SlipOrder(int OrderId,string OrderNumber,string MRNumber,string PatientName,DateTime OrderDate);
public record SlipItem(string Department,string Code,string TestName,string Status);