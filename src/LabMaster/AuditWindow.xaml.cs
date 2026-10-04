using System.Windows;
using LabMaster.Services;
namespace LabMaster;
public partial class AuditWindow:Window
{
 readonly AuditService service=new();
 public AuditWindow(){InitializeComponent();Loaded+=async(_,_)=>await LoadAsync();}
 async Task LoadAsync(){try{AuditGrid.ItemsSource=await service.GetAsync();}catch(Exception ex){MessageBox.Show(ex.Message,"Lab Master",MessageBoxButton.OK,MessageBoxImage.Error);}}
 async void Refresh_Click(object s,RoutedEventArgs e)=>await LoadAsync();
}