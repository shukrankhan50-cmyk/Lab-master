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
 public async Task AddAsync(string code,string name)
 {
  await using var c=Database.CreateConnection();await c.OpenAsync();
  const string sql="INSERT dbo.TestProfiles(ProfileCode,ProfileName) VALUES(@c,@n)";
  await using var cmd=new SqlCommand(sql,c);
  cmd.Parameters.AddWithValue("@c",code.Trim());cmd.Parameters.AddWithValue("@n",name.Trim());
  await cmd.ExecuteNonQueryAsync();
 }
 public async Task SetActiveAsync(int id,bool active)
 {
  await using var c=Database.CreateConnection();await c.OpenAsync();
  await using var cmd=new SqlCommand("UPDATE dbo.TestProfiles SET IsActive=@a WHERE ProfileId=@id",c);
  cmd.Parameters.AddWithValue("@a",active);cmd.Parameters.AddWithValue("@id",id);await cmd.ExecuteNonQueryAsync();
 }
 public async Task<List<ProfileTestRow>> GetItemsAsync(int profileId)
 {
  await using var c=Database.CreateConnection();await c.OpenAsync();
  const string sql="SELECT pi.ProfileItemId,t.TestId,t.TestCode,t.TestName,pi.DisplayOrder FROM dbo.TestProfileItems pi JOIN dbo.Tests t ON t.TestId=pi.TestId WHERE pi.ProfileId=@id ORDER BY pi.DisplayOrder,t.TestName";
  await using var cmd=new SqlCommand(sql,c);cmd.Parameters.AddWithValue("@id",profileId);await using var r=await cmd.ExecuteReaderAsync();var list=new List<ProfileTestRow>();while(await r.ReadAsync())list.Add(new ProfileTestRow(r.GetInt32(0),r.GetInt32(1),r.GetString(2),r.GetString(3),r.GetInt32(4)));return list;
 }
 public async Task AddTestAsync(int profileId,int testId,int displayOrder)
 {
  await using var c=Database.CreateConnection();await c.OpenAsync();const string sql="IF NOT EXISTS(SELECT 1 FROM dbo.TestProfileItems WHERE ProfileId=@p AND TestId=@t) INSERT dbo.TestProfileItems(ProfileId,TestId,DisplayOrder) VALUES(@p,@t,@o)";
  await using var cmd=new SqlCommand(sql,c);cmd.Parameters.AddWithValue("@p",profileId);cmd.Parameters.AddWithValue("@t",testId);cmd.Parameters.AddWithValue("@o",displayOrder);await cmd.ExecuteNonQueryAsync();
 }
 public async Task RemoveTestAsync(int profileItemId)
 {
  await using var c=Database.CreateConnection();await c.OpenAsync();await using var cmd=new SqlCommand("DELETE FROM dbo.TestProfileItems WHERE ProfileItemId=@id",c);cmd.Parameters.AddWithValue("@id",profileItemId);await cmd.ExecuteNonQueryAsync();
 }
}
public record ProfileRow(int ProfileId,string ProfileCode,string ProfileName,bool IsActive);\npublic record ProfileTestRow(int ProfileItemId,int TestId,string TestCode,string TestName,int DisplayOrder);