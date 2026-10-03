using System.Windows; using LabMaster.Services;
namespace LabMaster;
public partial class PatientSearchWindow : Window
{
 readonly PatientSearchService service=new();
 public PatientSearchWindow(){InitializeComponent(); Loaded+=async(_,_)=>await SearchAsync("");}
 private async void Search_Click(object sender,RoutedEventArgs e)=>await SearchAsync(SearchBox.Text);
 private async Task SearchAsync(string term){try{PatientsGrid.ItemsSource=await service.SearchAsync(term);}catch(Exception ex){MessageBox.Show(ex.Message,"Lab Master",MessageBoxButton.OK,MessageBoxImage.Error);}}
 private void PatientsGrid_SelectionChanged(object sender,System.Windows.Controls.SelectionChangedEventArgs e){}
 private void Close_Click(object sender,RoutedEventArgs e)=>Close();
}