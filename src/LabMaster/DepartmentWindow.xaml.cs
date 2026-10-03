using System.Windows;
using LabMaster.Data;
using Microsoft.Data.SqlClient;
namespace LabMaster;
public partial class DepartmentWindow:Window
{
 public DepartmentWindow(){InitializeComponent();Loaded+=async(_,_)=>await LoadAsync();}
 async Task LoadAsync(){await using var c=Database.CreateConnection();await c.OpenAsync();await using var cmd=new SqlCommand("SELECT DepartmentId,DepartmentName,IsActive FROM dbo.Departments ORDER BY DepartmentName",c);await using var r=await cmd.ExecuteReaderAsync();var list=new List<DepartmentRow>();while(await r.ReadAsync())list.Add(new DepartmentRow(r.GetInt32(0),r.GetString(1),r.GetBoolean(2)));Grid.ItemsSource=list;}
}
public record DepartmentRow(int DepartmentId,string DepartmentName,bool IsActive);