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
  await tx.CommitAsync();return no;
 }
}
public record BillItem(int OrderId,string OrderNumber,string MRNumber,string PatientName,decimal Total,decimal Paid,decimal Balance);