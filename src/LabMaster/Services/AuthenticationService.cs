using LabMaster.Data;
using Microsoft.Data.SqlClient;
namespace LabMaster.Services;
public sealed class AuthenticationService
{
 public async Task<bool> DatabaseAvailableAsync(){try{await using var c=Database.CreateConnection();await c.OpenAsync();return true;}catch{return false;}}
 public async Task<CurrentUser?> LoginAsync(string userName,string password)
 {
  await using var c=Database.CreateConnection();await c.OpenAsync();
  const string sql="SELECT TOP 1 UserId,UserName,DisplayName,RoleName,PasswordHash FROM dbo.Users WHERE UserName=@u AND IsActive=1";
  await using var cmd=new SqlCommand(sql,c);cmd.Parameters.AddWithValue("@u",userName.Trim());
  await using var r=await cmd.ExecuteReaderAsync();if(!await r.ReadAsync())return null;
  var hash=r.GetString(4);
  if(hash!=password)return null;
  return new CurrentUser(r.GetInt32(0),r.GetString(1),r.GetString(2),r.GetString(3));
 }
}