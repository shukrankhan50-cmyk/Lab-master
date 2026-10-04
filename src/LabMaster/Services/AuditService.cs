using LabMaster.Data;
using Microsoft.Data.SqlClient;
namespace LabMaster.Services;
public sealed class AuditService
{
 public async Task WriteAsync(string? user,string action,string? entity=null,int? entityId=null,string? details=null)
 {
  await using var c=Database.CreateConnection();await c.OpenAsync();
  const string s="INSERT dbo.AuditLogs(UserName,ActionName,EntityName,EntityId,Details) VALUES(@u,@a,@e,@i,@d)";
  await using var cmd=new SqlCommand(s,c);
  cmd.Parameters.AddWithValue("@u",(object?)user??DBNull.Value);cmd.Parameters.AddWithValue("@a",action);
  cmd.Parameters.AddWithValue("@e",(object?)entity??DBNull.Value);cmd.Parameters.AddWithValue("@i",(object?)entityId??DBNull.Value);cmd.Parameters.AddWithValue("@d",(object?)details??DBNull.Value);
  await cmd.ExecuteNonQueryAsync();
 }
 public async Task<List<AuditRow>> GetAsync(int take=200)
 {
  await using var c=Database.CreateConnection();await c.OpenAsync();
  const string s="SELECT TOP (@take) AuditId,UserName,ActionName,EntityName,EntityId,Details,CreatedAt FROM dbo.AuditLogs ORDER BY AuditId DESC";
  await using var cmd=new SqlCommand(s,c);cmd.Parameters.AddWithValue("@take",take);
  await using var r=await cmd.ExecuteReaderAsync();var x=new List<AuditRow>();
  while(await r.ReadAsync())x.Add(new AuditRow(r.GetInt32(0),r.IsDBNull(1)?null:r.GetString(1),r.GetString(2),r.IsDBNull(3)?null:r.GetString(3),r.IsDBNull(4)?null:r.GetInt32(4),r.IsDBNull(5)?null:r.GetString(5),r.GetDateTime(6)));
  return x;
 }
}
public record AuditRow(int Id,string? User,string Action,string? Entity,int? EntityId,string? Details,DateTime CreatedAt);