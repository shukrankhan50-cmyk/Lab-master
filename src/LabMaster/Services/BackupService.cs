using LabMaster.Data;
using Microsoft.Data.SqlClient;
namespace LabMaster.Services;
public sealed class BackupService
{
 public async Task BackupAsync(string filePath)
 {
  var builder=new SqlConnectionStringBuilder(Database.ConnectionString);
  var database=builder.InitialCatalog;
  await using var c=Database.CreateConnection();await c.OpenAsync();
  var sql=$"BACKUP DATABASE [{database}] TO DISK=@path WITH INIT, FORMAT;";
  await using var cmd=new SqlCommand(sql,c);cmd.Parameters.AddWithValue("@path",filePath);await cmd.ExecuteNonQueryAsync();
 }
}