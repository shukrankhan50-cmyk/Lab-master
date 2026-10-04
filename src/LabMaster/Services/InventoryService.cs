using LabMaster.Data;
using Microsoft.Data.SqlClient;
namespace LabMaster.Services;
public sealed class InventoryService
{
 public async Task<List<InventoryRow>> GetAsync(){await using var c=Database.CreateConnection();await c.OpenAsync();const string s="SELECT InventoryItemId,ItemName,Category,Unit,BatchNumber,ExpiryDate,Quantity,MinimumStock,Supplier,IsActive FROM dbo.InventoryItems ORDER BY ItemName";await using var cmd=new SqlCommand(s,c);await using var r=await cmd.ExecuteReaderAsync();var x=new List<InventoryRow>();while(await r.ReadAsync())x.Add(new InventoryRow(r.GetInt32(0),r.GetString(1),r.IsDBNull(2)?null:r.GetString(2),r.IsDBNull(3)?null:r.GetString(3),r.IsDBNull(4)?null:r.GetString(4),r.IsDBNull(5)?null:r.GetDateTime(5),r.GetDecimal(6),r.GetDecimal(7),r.IsDBNull(8)?null:r.GetString(8),r.GetBoolean(9)));return x;}
 public async Task AddAsync(string name,string? category,string? unit,string? batch,DateTime? expiry,decimal quantity,decimal minimum,string? supplier){await using var c=Database.CreateConnection();await c.OpenAsync();const string s="INSERT dbo.InventoryItems(ItemName,Category,Unit,BatchNumber,ExpiryDate,Quantity,MinimumStock,Supplier) VALUES(@n,@c,@u,@b,@e,@q,@m,@s)";await using var cmd=new SqlCommand(s,c);cmd.Parameters.AddWithValue("@n",name.Trim());cmd.Parameters.AddWithValue("@c",(object?)category??DBNull.Value);cmd.Parameters.AddWithValue("@u",(object?)unit??DBNull.Value);cmd.Parameters.AddWithValue("@b",(object?)batch??DBNull.Value);cmd.Parameters.AddWithValue("@e",(object?)expiry??DBNull.Value);cmd.Parameters.AddWithValue("@q",quantity);cmd.Parameters.AddWithValue("@m",minimum);cmd.Parameters.AddWithValue("@s",(object?)supplier??DBNull.Value);await cmd.ExecuteNonQueryAsync();}
 public async Task AdjustStockAsync(int itemId, decimal quantity, string type, string? reference, string? notes)
 {
  if (quantity <= 0) throw new InvalidOperationException("Quantity must be greater than zero.");
  if (type != "IN" && type != "OUT") throw new InvalidOperationException("Invalid stock transaction.");
  await using var c=Database.CreateConnection(); await c.OpenAsync(); await using var tx=await c.BeginTransactionAsync();
  const string check="SELECT Quantity FROM dbo.InventoryItems WITH(UPDLOCK) WHERE InventoryItemId=@id AND IsActive=1";
  await using var q=new SqlCommand(check,c,(SqlTransaction)tx); q.Parameters.AddWithValue("@id",itemId);
  var current=Convert.ToDecimal(await q.ExecuteScalarAsync());
  if(type=="OUT" && quantity>current) throw new InvalidOperationException("Stock cannot go below zero.");
  var delta=type=="IN"?quantity:-quantity;
  await using(var u=new SqlCommand("UPDATE dbo.InventoryItems SET Quantity=Quantity+@q WHERE InventoryItemId=@id",c,(SqlTransaction)tx)){u.Parameters.AddWithValue("@q",delta);u.Parameters.AddWithValue("@id",itemId);await u.ExecuteNonQueryAsync();}
  await using(var t=new SqlCommand("INSERT dbo.InventoryTransactions(InventoryItemId,TransactionType,Quantity,ReferenceNo,Notes,CreatedBy) VALUES(@id,@type,@q,@ref,@notes,@user)",c,(SqlTransaction)tx)){t.Parameters.AddWithValue("@id",itemId);t.Parameters.AddWithValue("@type",type);t.Parameters.AddWithValue("@q",quantity);t.Parameters.AddWithValue("@ref",(object?)reference??DBNull.Value);t.Parameters.AddWithValue("@notes",(object?)notes??DBNull.Value);t.Parameters.AddWithValue("@user",CurrentUserContext.UserName);await t.ExecuteNonQueryAsync();}
  await tx.CommitAsync();
  await new AuditService().WriteAsync(CurrentUserContext.UserName,"INVENTORY_"+type,"InventoryItem",itemId,$"Quantity {quantity:0.##}");
 }
 public async Task<List<InventoryTransactionRow>> GetTransactionsAsync(int itemId)
 {
  await using var c=Database.CreateConnection();await c.OpenAsync();
  const string s="SELECT TransactionId,TransactionType,Quantity,ReferenceNo,Notes,CreatedAt,CreatedBy FROM dbo.InventoryTransactions WHERE InventoryItemId=@id ORDER BY TransactionId DESC";
  await using var cmd=new SqlCommand(s,c);cmd.Parameters.AddWithValue("@id",itemId);await using var r=await cmd.ExecuteReaderAsync();var x=new List<InventoryTransactionRow>();
  while(await r.ReadAsync())x.Add(new InventoryTransactionRow(r.GetInt32(0),r.GetString(1),r.GetDecimal(2),r.IsDBNull(3)?null:r.GetString(3),r.IsDBNull(4)?null:r.GetString(4),r.GetDateTime(5),r.IsDBNull(6)?null:r.GetString(6)));
  return x;
 }
 public async Task<List<InventoryAlert>> GetAlertsAsync()
 {
  await using var c=Database.CreateConnection();await c.OpenAsync();
  const string s="SELECT InventoryItemId,ItemName,Quantity,MinimumStock,ExpiryDate FROM dbo.InventoryItems WHERE IsActive=1 AND (Quantity<=MinimumStock OR (ExpiryDate IS NOT NULL AND ExpiryDate<=DATEADD(DAY,30,CAST(GETDATE() AS DATE)))) ORDER BY ExpiryDate,ItemName";
  await using var cmd=new SqlCommand(s,c);await using var r=await cmd.ExecuteReaderAsync();var x=new List<InventoryAlert>();
  while(await r.ReadAsync())x.Add(new InventoryAlert(r.GetInt32(0),r.GetString(1),r.GetDecimal(2),r.GetDecimal(3),r.IsDBNull(4)?null:r.GetDateTime(4)));
  return x;
 }
}
public record InventoryRow(int Id,string Name,string? Category,string? Unit,string? Batch,DateTime? Expiry,decimal Quantity,decimal Minimum,string? Supplier,bool Active);
public record InventoryTransactionRow(int Id,string Type,decimal Quantity,string? Reference,string? Notes,DateTime CreatedAt,string? CreatedBy);
public record InventoryAlert(int Id,string Name,decimal Quantity,decimal Minimum,DateTime? Expiry);