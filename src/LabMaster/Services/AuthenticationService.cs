using System.Security.Cryptography;
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
  if(!VerifyPassword(password,r.GetString(4)))return null;
  return new CurrentUser(r.GetInt32(0),r.GetString(1),r.GetString(2),r.GetString(3));
 }
 public static string HashPassword(string password)
 {
  var salt=RandomNumberGenerator.GetBytes(16);
  var hash=Rfc2898DeriveBytes.Pbkdf2(password,salt,120000,HashAlgorithmName.SHA256,32);
  return "PBKDF2-SHA256$120000$"+Convert.ToBase64String(salt)+"$"+Convert.ToBase64String(hash);
 }
 static bool VerifyPassword(string password,string stored)
 {
  if(!stored.StartsWith("PBKDF2-SHA256$")) return stored==password;
  var p=stored.Split('$');if(p.Length!=4||!int.TryParse(p[1],out var iter))return false;
  var salt=Convert.FromBase64String(p[2]);var expected=Convert.FromBase64String(p[3]);
  var actual=Rfc2898DeriveBytes.Pbkdf2(password,salt,iter,HashAlgorithmName.SHA256,expected.Length);
  return CryptographicOperations.FixedTimeEquals(actual,expected);
 }
}