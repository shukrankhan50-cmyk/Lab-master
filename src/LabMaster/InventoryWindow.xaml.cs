using System.Windows;
using LabMaster.Services;
namespace LabMaster;
public partial class InventoryWindow:Window
{
 readonly InventoryService service=new();
 public InventoryWindow(){InitializeComponent();Loaded+=async(_,_)=>await LoadAsync();}
 async Task LoadAsync(){try{Grid.ItemsSource=await service.GetAsync();}catch(Exception ex){MessageBox.Show(ex.Message);}}
 async void Add_Click(object s,RoutedEventArgs e){var w=new AddInventoryWindow{Owner=this};if(w.ShowDialog()==true)await LoadAsync();}
}