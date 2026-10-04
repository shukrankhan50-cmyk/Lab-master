using LabMaster.Data;
using Microsoft.Data.SqlClient;
namespace LabMaster.Services;
public sealed class DepartmentService
{
 public async Task<List<DepartmentRow>> GetAsync()
 {
  await using var c=Database.CreateConnection();await c.OpenAsync();
  const string s="SELECT DepartmentId,DepartmentName,IsActive FROM dbo.Departments ORDER BY DepartmentName";
  await using var cmd=new SqlCommand(s,c);await using var r=await cmd.ExecuteReaderAsync();
  var x=new List<DepartmentRow>();while(await r.ReadAsync())x.Add(new DepartmentRow(r.GetInt32(0),r.GetString(1),r.GetBoolean(2)));return x;
 }
 public async Task AddAsync(string name)
 {
  await using var c=Database.CreateConnection();await c.OpenAsync();
  await using var cmd=new SqlCommand("INSERT dbo.Departments(DepartmentName) VALUES(@n)",c);
  cmd.Parameters.AddWithValue("@n",name.Trim());await cmd.ExecuteNonQueryAsync();
 }
 public async Task SetActiveAsync(int id,bool active)
 {
  await using var c=Database.CreateConnection();await c.OpenAsync();
  await using var cmd=new SqlCommand("UPDATE dbo.Departments SET IsActive=@a WHERE DepartmentId=@id",c);
  cmd.Parameters.AddWithValue("@a",active);cmd.Parameters.AddWithValue("@id",id);await cmd.ExecuteNonQueryAsync();
 }
}
public record DepartmentRow(int DepartmentId,string DepartmentName,bool IsActive);