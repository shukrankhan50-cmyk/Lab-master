using System.Windows;
using LabMaster.Data;
using Microsoft.Data.SqlClient;
namespace LabMaster;
public partial class SettingsWindow:Window
{
 public SettingsWindow(){InitializeComponent();Loaded+=async(_,_)=>await LoadAsync();}
 async Task LoadAsync(){await using var c=Database.CreateConnection();await c.OpenAsync();await using var cmd=new SqlCommand("SELECT TOP 1 LaboratoryName,Address,Phone,ReportFooter FROM dbo.ReportSettings ORDER BY SettingId DESC",c);await using var r=await cmd.ExecuteReaderAsync();if(await r.ReadAsync()){NameBox.Text=r.GetString(0);AddressBox.Text=r.IsDBNull(1)?"":r.GetString(1);PhoneBox.Text=r.IsDBNull(2)?"":r.GetString(2);FooterBox.Text=r.IsDBNull(3)?"":r.GetString(3);}}
 async void Save_Click(object s,RoutedEventArgs e){await using var c=Database.CreateConnection();await c.OpenAsync();const string q="UPDATE dbo.ReportSettings SET LaboratoryName=@n,Address=@a,Phone=@p,ReportFooter=@f WHERE SettingId=(SELECT TOP 1 SettingId FROM dbo.ReportSettings ORDER BY SettingId DESC)";await using var cmd=new SqlCommand(q,c);cmd.Parameters.AddWithValue("@n",NameBox.Text.Trim());cmd.Parameters.AddWithValue("@a",AddressBox.Text.Trim());cmd.Parameters.AddWithValue("@p",PhoneBox.Text.Trim());cmd.Parameters.AddWithValue("@f",FooterBox.Text.Trim());await cmd.ExecuteNonQueryAsync();MessageBox.Show("Settings saved.");}
}