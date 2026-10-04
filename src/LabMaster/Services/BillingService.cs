using LabMaster.Data;
using Microsoft.Data.SqlClient;
namespace LabMaster.Services;
public sealed class BillingService
{
 public async Task<List<BillItem>> GetPendingBillsAsync()
 {
  await using var c=Database.CreateConnection();await c.OpenAsync();
  const string sql="SELECT o.OrderId,o.OrderNumber,p.MRNumber,p.PatientName,o.TotalAmount,o.PaidAmount,(o.TotalAmount-o.PaidAmount) AS Balance FROM dbo.TestOrders o JOIN dbo.Patients p ON p.PatientId=o.PatientId WHERE o.TotalAmount>o.PaidAmount ORDER BY o.OrderId DESC";
  await using var cmd=new SqlCommand(sql,c);await using var r=await cmd.ExecuteReaderAsync();var list=new List<BillItem>();
  while(await r.ReadAsync())list.Add(new BillItem(r.GetInt32(0),r.GetString(1),r.GetString(2),r.GetString(3),r.GetDecimal(4),r.GetDecimal(5),r.GetDecimal(6)));
  return list;
 }
 public async Task<string> ReceiveAsync(int orderId,decimal amount,string method,string receiver)
 {
  await using var c=Database.CreateConnection();await c.OpenAsync();await using var tx=await c.BeginTransactionAsync();
  const string check="SELECT TotalAmount,PaidAmount FROM dbo.TestOrders WITH(UPDLOCK) WHERE OrderId=@id";
  await using var cc=new SqlCommand(check,c,(SqlTransaction)tx);cc.Parameters.AddWithValue("@id",orderId);await using var r=await cc.ExecuteReaderAsync();
  if(!await r.ReadAsync())throw new InvalidOperationException("Order not found.");
  var total=r.GetDecimal(0);var paid=r.GetDecimal(1);await r.CloseAsync();
  if(amount<=0 || paid+amount>total)throw new InvalidOperationException("Payment amount is invalid.");
  var no="RCPT-"+DateTime.Now.ToString("yyyyMMddHHmmssfff");
  await using(var cmd=new SqlCommand("INSERT dbo.Payments(OrderId,ReceiptNumber,Amount,PaymentMethod,ReceivedBy) VALUES(@o,@n,@a,@m,@u); UPDATE dbo.TestOrders SET PaidAmount=PaidAmount+@a WHERE OrderId=@o;",c,(SqlTransaction)tx))
  {cmd.Parameters.AddWithValue("@o",orderId);cmd.Parameters.AddWithValue("@n",no);cmd.Parameters.AddWithValue("@a",amount);cmd.Parameters.AddWithValue("@m",method);cmd.Parameters.AddWithValue("@u",receiver);await cmd.ExecuteNonQueryAsync();}
  await tx.CommitAsync();
  await new AuditService().WriteAsync(CurrentUserContext.UserName, "PAYMENT", "Payment", null, $"Receipt {no}, Order {orderId}, Amount {amount:0.00}");
  return no;
 }
 public async Task<List<ReceiptRow>> GetReceiptsAsync()
 {
  await using var c=Database.CreateConnection();await c.OpenAsync();
  const string sql="SELECT TOP 200 p.PaymentId,p.ReceiptNumber,p.OrderId,o.OrderNumber,pt.MRNumber,pt.PatientName,p.Amount,p.PaymentMethod,p.PaidAt,p.ReceivedBy FROM dbo.Payments p JOIN dbo.TestOrders o ON o.OrderId=p.OrderId JOIN dbo.Patients pt ON pt.PatientId=o.PatientId ORDER BY p.PaymentId DESC";
  await using var cmd=new SqlCommand(sql,c);await using var r=await cmd.ExecuteReaderAsync();var x=new List<ReceiptRow>();
  while(await r.ReadAsync())x.Add(new ReceiptRow(r.GetInt32(0),r.GetString(1),r.GetInt32(2),r.GetString(3),r.GetString(4),r.GetString(5),r.GetDecimal(6),r.GetString(7),r.GetDateTime(8),r.IsDBNull(9)?null:r.GetString(9)));
  return x;
 }
}
public record ReceiptRow(int PaymentId,string ReceiptNumber,int OrderId,string OrderNumber,string MRNumber,string PatientName,decimal Amount,string PaymentMethod,DateTime PaidAt,string? ReceivedBy);
public record BillItem(int OrderId,string OrderNumber,string MRNumber,string PatientName,decimal Total,decimal Paid,decimal Balance);