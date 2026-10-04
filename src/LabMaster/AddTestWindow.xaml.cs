using System.Windows;
using LabMaster.Services;
namespace LabMaster;
public partial class AddTestWindow:Window
{
 readonly TestMasterService service=new();
 public AddTestWindow(){InitializeComponent();Loaded+=async(_,_)=>DepartmentBox.ItemsSource=await service.GetDepartmentsAsync();}
 async void Save_Click(object s,RoutedEventArgs e){if(string.IsNullOrWhiteSpace(CodeBox.Text)||string.IsNullOrWhiteSpace(NameBox.Text)||DepartmentBox.SelectedItem is not DepartmentOption d||!decimal.TryParse(PriceBox.Text,out var price)){Status.Text="Code, name, department and valid price are required.";return;}try{await service.AddAsync(CodeBox.Text,NameBox.Text,d.Id,SampleBox.Text,UnitBox.Text,RangeBox.Text,price);DialogResult=true;}catch(Exception ex){Status.Text=ex.Message;}}
}