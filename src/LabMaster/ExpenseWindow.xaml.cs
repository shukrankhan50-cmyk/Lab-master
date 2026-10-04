using System.Windows;
using LabMaster.Services;
namespace LabMaster;
public partial class ExpenseWindow:Window
{
 readonly ExpenseService service=new();
 public ExpenseWindow(){InitializeComponent();Loaded+=async(_,_)=>await LoadAsync();}
 async Task LoadAsync(){try{Grid.ItemsSource=await service.GetAsync(DateTime.Today.AddDays(-30),DateTime.Today);}catch(Exception ex){MessageBox.Show(ex.Message);}}
 async void Add_Click(object s,RoutedEventArgs e){var w=new AddExpenseWindow{Owner=this};if(w.ShowDialog()==true)await LoadAsync();}
}