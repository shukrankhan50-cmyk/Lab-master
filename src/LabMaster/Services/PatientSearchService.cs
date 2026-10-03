using LabMaster.Data;
using LabMaster.Models;
using Microsoft.Data.SqlClient;
namespace LabMaster.Services;
public sealed class PatientSearchService
{
 public async Task<List<PatientSearchResult>> SearchAsync(string term)
 {
  await using var c=Database.CreateConnection(); await c.OpenAsync();
  const string sql="SELECT TOP 50 PatientId,MRNumber,PatientName,Gender,Phone FROM dbo.Patients WHERE IsActive=1 AND (MRNumber LIKE @q OR PatientName LIKE @q OR Phone LIKE @q) ORDER BY PatientId DESC";
  await using var cmd=new SqlCommand(sql,c); cmd.Parameters.AddWithValue("@q","%"+term.Trim()+"%");
  var list=new List<PatientSearchResult>();
  await using var r=await cmd.ExecuteReaderAsync();
  while(await r.ReadAsync()) list.Add(new PatientSearchResult{PatientId=r.GetInt32(0),MRNumber=r.GetString(1),PatientName=r.GetString(2),Gender=r.GetString(3),Phone=r.IsDBNull(4)?null:r.GetString(4)});
  return list;
 }
}