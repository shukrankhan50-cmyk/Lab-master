using LabMaster.Data;
using Microsoft.Data.SqlClient;
namespace LabMaster.Services;
public sealed class DashboardService
{
 public async Task<DashboardStats> GetAsync()
 {
  await using var c=Database.CreateConnection();
  await c.OpenAsync();
  const string sql="SELECT (SELECT COUNT(*) FROM dbo.Patients WHERE CAST(CreatedAt AS date)=CAST(SYSDATETIME() AS date)), (SELECT COUNT(*) FROM dbo.TestOrderItems WHERE Status IN ('Pending','Entered')), (SELECT COUNT(*) FROM dbo.TestOrders WHERE Status='Verified'), (SELECT ISNULL(SUM(Amount),0) FROM dbo.Payments WHERE CAST(PaidAt AS date)=CAST(SYSDATETIME() AS date));";
  await using var cmd=new SqlCommand(sql,c);
  await using var r=await cmd.ExecuteReaderAsync();
  if(!await r.ReadAsync()) return new DashboardStats(0,0,0,0);
  return new DashboardStats(r.GetInt32(0),r.GetInt32(1),r.GetInt32(2),r.GetDecimal(3));
 }
}
public record DashboardStats(int TodayPatients,int PendingTests,int ReportsReady,decimal TodayCollection);