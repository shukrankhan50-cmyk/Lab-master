using System.Windows;
using System.Windows.Controls;
using LabMaster.Services;
namespace LabMaster;
public partial class ReportWindow : Window
{
 readonly ReportService service=new(); ReportOrder? selected; ReportData? report;
 public ReportWindow(){InitializeComponent();Loaded+=async(_,_)=>await LoadAsync();}
 async Task LoadAsync(){try{OrdersGrid.ItemsSource=await service.GetEnteredOrdersAsync();}catch(Exception ex){MessageBox.Show(ex.Message,"Lab Master",MessageBoxButton.OK,MessageBoxImage.Error);}}
 async void OrdersGrid_SelectionChanged(object s,SelectionChangedEventArgs e){try{selected=OrdersGrid.SelectedItem as ReportOrder;if(selected==null)return;await service.EnsureReportNumberAsync(selected.OrderId);report=await service.GetReportAsync(selected.OrderId);if(report==null)return;PatientInfo.Text=$"Report: {report.ReportNumber}    Order: {report.OrderNumber}    MR: {report.MRNumber}\nPatient: {report.PatientName}    Gender: {report.Gender}    Date: {report.OrderDate:dd-MMM-yyyy HH:mm}";ReportGrid.ItemsSource=report.Results;CommentText.Text=string.Join(" | ",report.Results.Where(x=>!string.IsNullOrWhiteSpace(x.Comment)).Select(x=>x.Comment));VerifiedText.Text=report.VerifiedBy==null?"Pending Verification":$"Verified by: {report.VerifiedBy}  |  {report.VerifiedAt:dd-MMM-yyyy HH:mm}";}catch(Exception ex){MessageBox.Show(ex.Message,"Lab Master",MessageBoxButton.OK,MessageBoxImage.Error);}}
 async void Verify_Click(object s,RoutedEventArgs e){if(selected==null){MessageBox.Show("Select a report.");return;}try{await service.VerifyAsync(selected.OrderId,CurrentUserContext.UserName);MessageBox.Show("Report verified successfully.");await LoadAsync();}catch(Exception ex){MessageBox.Show(ex.Message,"Lab Master",MessageBoxButton.OK,MessageBoxImage.Error);}}
 void Print_Click(object s,RoutedEventArgs e){if(report==null){MessageBox.Show("Select a report first.");return;}var dlg=new PrintDialog();if(dlg.ShowDialog()!=true)return;dlg.PrintVisual(ReportPanel,$"Lab Report - {report.ReportNumber ?? report.MRNumber}");}
}