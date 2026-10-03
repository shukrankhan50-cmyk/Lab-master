using System.Windows;
using System.Windows.Controls;
using System.Printing;
using System.Windows.Documents;
using LabMaster.Services;
namespace LabMaster;
public partial class ReportWindow : Window
{
 readonly ReportService service=new(); ReportOrder? selected; ReportData? report;
 public ReportWindow(){InitializeComponent();Loaded+=async(_,_)=>{try{OrdersGrid.ItemsSource=await service.GetEnteredOrdersAsync();}catch(Exception ex){MessageBox.Show(ex.Message);}};}
 async void OrdersGrid_SelectionChanged(object s,SelectionChangedEventArgs e){selected=OrdersGrid.SelectedItem as ReportOrder;if(selected==null)return;report=await service.GetReportAsync(selected.OrderId);if(report==null)return;PatientInfo.Text=$"Order: {report.OrderNumber}    MR: {report.MRNumber}\nPatient: {report.PatientName}    Gender: {report.Gender}    Date: {report.OrderDate:dd-MMM-yyyy HH:mm}";ReportGrid.ItemsSource=report.Results;CommentText.Text=string.Join(" | ",report.Results.Where(x=>!string.IsNullOrWhiteSpace(x.Comment)).Select(x=>x.Comment));}
 async void Verify_Click(object s,RoutedEventArgs e){if(selected==null){MessageBox.Show("Select a report.");return;}await service.VerifyAsync(selected.OrderId,"Admin");MessageBox.Show("Report verified.");OrdersGrid.ItemsSource=await service.GetEnteredOrdersAsync();}
 void Print_Click(object s,RoutedEventArgs e){if(report==null){MessageBox.Show("Select a report first.");return;}var dlg=new PrintDialog();if(dlg.ShowDialog()!=true)return;dlg.PrintVisual(ReportPanel,$"Lab Report - {report.MRNumber}");}
}