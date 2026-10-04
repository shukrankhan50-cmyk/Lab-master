using System.Windows;
using LabMaster.Services;
namespace LabMaster;
public partial class TestMasterWindow : Window
{
 readonly TestMasterService service=new();
 public TestMasterWindow(){InitializeComponent();Loaded+=async(_,_)=>await LoadAsync("");}
 async Task LoadAsync(string q){try{TestsGrid.ItemsSource=await service.GetAsync(q);}catch(Exception ex){MessageBox.Show(ex.Message,"Lab Master",MessageBoxButton.OK,MessageBoxImage.Error);}}
 async void Search_TextChanged(object s,System.Windows.Controls.TextChangedEventArgs e){if(IsLoaded)await LoadAsync(SearchBox.Text);}
 async void Add_Click(object s,RoutedEventArgs e){var w=new AddTestWindow{Owner=this};if(w.ShowDialog()==true)await LoadAsync(SearchBox.Text);}

}