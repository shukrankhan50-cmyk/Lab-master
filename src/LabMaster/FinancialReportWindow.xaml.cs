using System.Windows;
using LabMaster.Services;
namespace LabMaster;
public partial class FinancialReportWindow:Window
{
 readonly FinancialReportService service=new();
 public FinancialReportWindow(){InitializeComponent();FromBox.SelectedDate=DateTime.Today;ToBox.SelectedDate=DateTime.Today;}
 async void Refresh_Click(object s,RoutedEventArgs e){try{var x=await service.GetAsync(FromBox.SelectedDate??DateTime.Today,ToBox.SelectedDate??DateTime.Today);CollectionText.Text=$"Collection: PKR {x.Collection:N2}";ExpenseText.Text=$"Expenses: PKR {x.Expenses:N2}";NetText.Text=$"Net: PKR {x.Net:N2}";}catch(Exception ex){MessageBox.Show(ex.Message);}}
}