using System.Windows;
using LabMaster.Services;
namespace LabMaster;
public partial class TestMasterWindow : Window
{
 readonly TestMasterService service=new();
 public TestMasterWindow(){InitializeComponent();Loaded+=async(_,_)=>await LoadAsync("");}
 async Task LoadAsync(string q){try{TestsGrid.ItemsSource=await service.GetAsync(q);}catch(Exception ex){MessageBox.Show(ex.Message,"Lab Master",MessageBoxButton.OK,MessageBoxImage.Error);}}
 async void Search_TextChanged(object s,System.Windows.Controls.TextChangedEventArgs e){if(IsLoaded)await LoadAsync(SearchBox.Text);}
 async void Add_Click(object s,RoutedEventArgs e)
 {
  var d=await service.GetDepartmentsAsync(); if(d.Count==0){MessageBox.Show("Create a department first.");return;}
  var code=Microsoft.VisualBasic.Interaction.InputBox("Test code:","Add Test","NEW-TEST");
  if(string.IsNullOrWhiteSpace(code))return;
  var name=Microsoft.VisualBasic.Interaction.InputBox("Test name:","Add Test","New Laboratory Test");
  if(string.IsNullOrWhiteSpace(name))return;
  var dept=d[0]; var priceText=Microsoft.VisualBasic.Interaction.InputBox($"Price (department: {dept.Name}):","Add Test","0");
  if(!decimal.TryParse(priceText,out var price))price=0;
  try{await service.AddAsync(code.Trim(),name.Trim(),dept.Id,null,null,null,price);await LoadAsync(SearchBox.Text);MessageBox.Show("Test added.");}catch(Exception ex){MessageBox.Show(ex.Message,"Lab Master",MessageBoxButton.OK,MessageBoxImage.Error);}
 }
}