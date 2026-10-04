using System.Windows;
using LabMaster.Services;
namespace LabMaster;
public partial class DepartmentWindow:Window
{
 readonly DepartmentService service=new();
 public DepartmentWindow(){InitializeComponent();Loaded+=async(_,_)=>await LoadAsync();}
 async Task LoadAsync(){try{Grid.ItemsSource=await service.GetAsync();}catch(Exception ex){MessageBox.Show(ex.Message,"Lab Master",MessageBoxButton.OK,MessageBoxImage.Error);}}
 async void Add_Click(object sender,RoutedEventArgs e){var w=new AddDepartmentWindow{Owner=this};if(w.ShowDialog()==true)await LoadAsync();}
}