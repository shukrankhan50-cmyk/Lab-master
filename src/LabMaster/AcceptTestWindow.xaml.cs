using System.Windows;
using LabMaster.Services;
namespace LabMaster;
public partial class AcceptTestWindow : Window
{
 readonly TestOrderService service=new(); int? patientId;
 public AcceptTestWindow(){InitializeComponent(); Loaded+=async(_,_)=>TestsGrid.ItemsSource=await service.GetTestsAsync();}
 async void Search_TextChanged(object s,System.Windows.Controls.TextChangedEventArgs e){TestsGrid.ItemsSource=await service.GetTestsAsync(TestSearchBox.Text);}\n async void Search_Click(object s,RoutedEventArgs e){TestsGrid.ItemsSource=await service.GetTestsAsync(TestSearchBox.Text);}
 async void FindPatient_Click(object s,RoutedEventArgs e){if(string.IsNullOrWhiteSpace(MrBox.Text)){MessageBox.Show("Enter MR number.");return;} var p=new PatientSearchService(); var list=await p.SearchAsync(MrBox.Text); var match=list.Find(x=>x.MRNumber.Equals(MrBox.Text.Trim(),StringComparison.OrdinalIgnoreCase)); if(match==null){MessageBox.Show("Patient not found.");return;} patientId=match.PatientId;PatientLabel.Text=$"{match.PatientName} | {match.Gender} | {match.Phone}"; }
 async void CreateOrder_Click(object s,RoutedEventArgs e){if(patientId==null){MessageBox.Show("Select a patient first.");return;} var selected=((System.Collections.IEnumerable)TestsGrid.ItemsSource).Cast<TestOption>().Where(x=>TestsGrid.SelectedItems.Contains(x)).ToList(); if(selected.Count==0){MessageBox.Show("Select at least one test.");return;} try{var no=await service.CreateOrderAsync(patientId.Value,selected.Select(x=>x.TestId));MessageBox.Show($"Order created successfully.\nOrder Number: {no}");Close();}catch(Exception ex){MessageBox.Show(ex.Message,"Lab Master",MessageBoxButton.OK,MessageBoxImage.Error);}}
}