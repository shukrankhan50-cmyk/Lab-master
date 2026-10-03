using System.Windows;
using System.Windows.Controls;
using LabMaster.Services;
namespace LabMaster;
public partial class ResultEntryWindow : Window
{
 readonly ResultEntryService service=new(); PendingResult? selected;
 public ResultEntryWindow(){InitializeComponent();Loaded+=async(_,_)=>await LoadAsync();}
 async Task LoadAsync(){try{ResultsGrid.ItemsSource=await service.GetPendingAsync();}catch(Exception ex){MessageBox.Show(ex.Message,"Lab Master",MessageBoxButton.OK,MessageBoxImage.Error);}}
 void Grid_SelectionChanged(object s,SelectionChangedEventArgs e)
 {
  selected=ResultsGrid.SelectedItem as PendingResult;
  if(selected!=null){SelectedTestLabel.Text=$"{selected.MRNumber} | {selected.PatientName} | {selected.TestName}";ResultBox.Text=selected.ResultValue??"";ResultBox.Focus();}
 }
 async void Save_Click(object s,RoutedEventArgs e)
 {
  if(selected==null){MessageBox.Show("Select a pending test.");return;}
  if(string.IsNullOrWhiteSpace(ResultBox.Text)){MessageBox.Show("Enter a result.");ResultBox.Focus();return;}
  var flag=ResultFlagService.Flag(ResultBox.Text,selected.CriticalLow,selected.CriticalHigh);
  if(!string.IsNullOrEmpty(flag) && flag!="NORMAL")
  {
   var answer=MessageBox.Show($"Result flag: {flag}. Save this result?","Lab Master",MessageBoxButton.YesNo,MessageBoxImage.Warning);
   if(answer!=MessageBoxResult.Yes)return;
  }
  try{await service.SaveResultAsync(selected.OrderItemId,ResultBox.Text.Trim(),string.IsNullOrWhiteSpace(CommentBox.Text)?null:CommentBox.Text.Trim());MessageBox.Show("Result saved.");await LoadAsync();ResultBox.Clear();CommentBox.Clear();SelectedTestLabel.Text="Select a pending test";selected=null;}catch(Exception ex){MessageBox.Show(ex.Message,"Lab Master",MessageBoxButton.OK,MessageBoxImage.Error);}
 }
}