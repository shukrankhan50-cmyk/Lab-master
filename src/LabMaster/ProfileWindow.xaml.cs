using System.Windows;
using LabMaster.Services;
namespace LabMaster;
public partial class ProfileWindow:Window
{
 readonly ProfileService service=new();
 public ProfileWindow(){InitializeComponent();Loaded+=async(_,_)=>await LoadAsync();}
 async Task LoadAsync(){try{Grid.ItemsSource=await service.GetAsync();}catch(Exception ex){MessageBox.Show(ex.Message,"Lab Master",MessageBoxButton.OK,MessageBoxImage.Error);}}
 async void Add_Click(object sender,RoutedEventArgs e){var w=new AddProfileWindow{Owner=this};if(w.ShowDialog()==true)await LoadAsync();}
}