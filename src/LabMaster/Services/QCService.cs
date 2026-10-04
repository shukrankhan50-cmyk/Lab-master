using LabMaster.Data;
using Microsoft.Data.SqlClient;
namespace LabMaster.Services;
public sealed class QCService
{
 public async Task<List<QCRow>> GetAsync(){await using var c=Database.CreateConnection();await c.OpenAsync();const string s="SELECT q.QCId,q.MaterialName,q.LotNumber,t.TestName,q.TargetMean,q.SD,q.ExpiryDate FROM dbo.QCMaterials q LEFT JOIN dbo.Tests t ON t.TestId=q.TestId ORDER BY q.MaterialName";await using var cmd=new SqlCommand(s,c);await using var r=await cmd.ExecuteReaderAsync();var x=new List<QCRow>();while(await r.ReadAsync())x.Add(new QCRow(r.GetInt32(0),r.GetString(1),r.IsDBNull(2)?null:r.GetString(2),r.IsDBNull(3)?null:r.GetString(3),r.IsDBNull(4)?null:r.GetDecimal(4),r.IsDBNull(5)?null:r.GetDecimal(5),r.IsDBNull(6)?null:r.GetDateTime(6)));return x;}
}
public record QCRow(int Id,string Material,string? Lot,string? Test,decimal? Mean,decimal? SD,DateTime? Expiry);