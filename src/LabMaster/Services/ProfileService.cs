using LabMaster.Data;
using Microsoft.Data.SqlClient;
namespace LabMaster.Services;
public sealed class ProfileService
{
 public async Task<List<ProfileRow>> GetAsync()
 {
  await using var c=Database.CreateConnection();await c.OpenAsync();
  const string sql="SELECT ProfileId,ProfileCode,ProfileName,IsActive FROM dbo.TestProfiles ORDER BY ProfileName";
  await using var cmd=new SqlCommand(sql,c);await using var r=await cmd.ExecuteReaderAsync();var list=new List<ProfileRow>();
  while(await r.ReadAsync())list.Add(new ProfileRow(r.GetInt32(0),r.GetString(1),r.GetString(2),r.GetBoolean(3)));
  return list;
 }
}
public record ProfileRow(int ProfileId,string ProfileCode,string ProfileName,bool IsActive);