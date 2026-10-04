using System.Windows;
using LabMaster.Services;
namespace LabMaster;
public partial class AddProfileWindow:Window
{
 readonly ProfileService service=new();
 public AddProfileWindow(){InitializeComponent();}
 async void Save_Click(object sender,RoutedEventArgs e)
 {
  if(string.IsNullOrWhiteSpace(CodeBox.Text)||string.IsNullOrWhiteSpace(NameBox.Text)){MessageBox.Show("Enter profile code and name.");return;}
  try{await service.AddAsync(CodeBox.Text,NameBox.Text);DialogResult=true;}
  catch(Exception ex){MessageBox.Show(ex.Message,"Lab Master",MessageBoxButton.OK,MessageBoxImage.Error);}
 }
}