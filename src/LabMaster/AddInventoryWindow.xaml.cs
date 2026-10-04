using System.Windows;
using LabMaster.Services;
namespace LabMaster;
public partial class AddInventoryWindow:Window
{
 readonly InventoryService service=new();
 public AddInventoryWindow(){InitializeComponent();}
 async void Save_Click(object s,RoutedEventArgs e){if(string.IsNullOrWhiteSpace(NameBox.Text)||!decimal.TryParse(QtyBox.Text,out var q)||!decimal.TryParse(MinBox.Text,out var m)){Status.Text="Item name and valid quantity/minimum are required.";return;}try{await service.AddAsync(NameBox.Text,CategoryBox.Text,UnitBox.Text,BatchBox.Text,ExpiryBox.SelectedDate,q,m,SupplierBox.Text);DialogResult=true;}catch(Exception ex){Status.Text=ex.Message;}}
}