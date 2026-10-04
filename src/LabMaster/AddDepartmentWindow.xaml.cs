using System.Windows;
using LabMaster.Services;
namespace LabMaster;
public partial class AddDepartmentWindow:Window
{
 readonly DepartmentService service=new();
 public AddDepartmentWindow(){InitializeComponent();}
 async void Save_Click(object sender,RoutedEventArgs e)
 {
  if(string.IsNullOrWhiteSpace(NameBox.Text)){MessageBox.Show("Enter department name.");return;}
  try{await service.AddAsync(NameBox.Text);DialogResult=true;}
  catch(Exception ex){MessageBox.Show(ex.Message,"Lab Master",MessageBoxButton.OK,MessageBoxImage.Error);}
 }
}