using LabMaster.Data;
using LabMaster.Models;
using Microsoft.Data.SqlClient;
namespace LabMaster.Services;
public sealed class TestMasterService
{
 public async Task<List<TestMasterItem>> GetAsync(string term="")
 {
  await using var c=Database.CreateConnection();await c.OpenAsync();
  const string sql="SELECT t.TestId,t.TestCode,t.TestName,t.DepartmentId,d.DepartmentName,t.SampleType,t.Unit,t.ReferenceRange,t.Price,t.IsActive FROM dbo.Tests t JOIN dbo.Departments d ON d.DepartmentId=t.DepartmentId WHERE t.TestCode LIKE @q OR t.TestName LIKE @q ORDER BY t.TestName";
  await using var cmd=new SqlCommand(sql,c);cmd.Parameters.AddWithValue("@q","%"+term.Trim()+"%");
  await using var r=await cmd.ExecuteReaderAsync();var list=new List<TestMasterItem>();
  while(await r.ReadAsync()) list.Add(new TestMasterItem{TestId=r.GetInt32(0),TestCode=r.GetString(1),TestName=r.GetString(2),DepartmentId=r.GetInt32(3),DepartmentName=r.GetString(4),SampleType=r.IsDBNull(5)?null:r.GetString(5),Unit=r.IsDBNull(6)?null:r.GetString(6),ReferenceRange=r.IsDBNull(7)?null:r.GetString(7),Price=r.GetDecimal(8),IsActive=r.GetBoolean(9)});
  return list;
 }
 public async Task AddAsync(string code,string name,int departmentId,string? sample,string? unit,string? reference,decimal price)
 {
  await using var c=Database.CreateConnection();await c.OpenAsync();
  const string sql="INSERT dbo.Tests(TestCode,TestName,DepartmentId,SampleType,Unit,ReferenceRange,Price) VALUES(@code,@name,@dept,@sample,@unit,@ref,@price)";
  await using var cmd=new SqlCommand(sql,c);
  cmd.Parameters.AddWithValue("@code",code);cmd.Parameters.AddWithValue("@name",name);cmd.Parameters.AddWithValue("@dept",departmentId);
  cmd.Parameters.AddWithValue("@sample",(object?)sample??DBNull.Value);cmd.Parameters.AddWithValue("@unit",(object?)unit??DBNull.Value);cmd.Parameters.AddWithValue("@ref",(object?)reference??DBNull.Value);cmd.Parameters.AddWithValue("@price",price);
  await cmd.ExecuteNonQueryAsync();
 }
 public async Task<List<(int Id,string Name)>> GetDepartmentsAsync()
 {
  await using var c=Database.CreateConnection();await c.OpenAsync();await using var cmd=new SqlCommand("SELECT DepartmentId,DepartmentName FROM dbo.Departments WHERE IsActive=1 ORDER BY DepartmentName",c);await using var r=await cmd.ExecuteReaderAsync();var x=new List<(int,string)>();while(await r.ReadAsync())x.Add((r.GetInt32(0),r.GetString(1)));return x;
 }
 public async Task UpdateAsync(int id,string code,string name,int departmentId,string? sample,string? unit,string? range,decimal price)
 {
  await using var c=Database.CreateConnection();await c.OpenAsync();
  const string sql="UPDATE dbo.Tests SET TestCode=@code,TestName=@name,DepartmentId=@dep,SampleType=@sample,Unit=@unit,ReferenceRange=@range,Price=@price WHERE TestId=@id";
  await using var cmd=new SqlCommand(sql,c);cmd.Parameters.AddWithValue("@id",id);cmd.Parameters.AddWithValue("@code",code.Trim());cmd.Parameters.AddWithValue("@name",name.Trim());cmd.Parameters.AddWithValue("@dep",departmentId);cmd.Parameters.AddWithValue("@sample",(object?)sample??DBNull.Value);cmd.Parameters.AddWithValue("@unit",(object?)unit??DBNull.Value);cmd.Parameters.AddWithValue("@range",(object?)range??DBNull.Value);cmd.Parameters.AddWithValue("@price",price);await cmd.ExecuteNonQueryAsync();
 }

}