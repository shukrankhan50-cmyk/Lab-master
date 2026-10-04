using System.Windows;
using LabMaster.Services;
namespace LabMaster;
public partial class BillingWindow : Window
{
 readonly BillingService service=new(); BillItem? selected;
 public BillingWindow(){InitializeComponent();Loaded+=async(_,_)=>await LoadAsync();}
 async Task LoadAsync(){BillsGrid.ItemsSource=await service.GetPendingBillsAsync();}
 void BillsGrid_SelectionChanged(object s,System.Windows.Controls.SelectionChangedEventArgs e){selected=BillsGrid.SelectedItem as BillItem;if(selected!=null){BalanceLabel.Text=$"Balance: {selected.Balance:N2}";AmountBox.Text=selected.Balance.ToString("0.00");}}
 async void Receive_Click(object s,RoutedEventArgs e){if(selected==null){MessageBox.Show("Select an order.");return;}if(!decimal.TryParse(AmountBox.Text,out var amount)){MessageBox.Show("Enter a valid amount.");return;}try{var no=await service.ReceiveAsync(selected.OrderId,amount,((System.Windows.Controls.ComboBoxItem)MethodBox.SelectedItem).Content.ToString()! ,CurrentUserContext.UserName);MessageBox.Show($"Payment received.\nReceipt: {no}");await LoadAsync();}catch(Exception ex){MessageBox.Show(ex.Message,"Lab Master",MessageBoxButton.OK,MessageBoxImage.Error);}}
}