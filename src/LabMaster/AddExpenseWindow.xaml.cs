using System.Windows;
using LabMaster.Services;
namespace LabMaster;
public partial class AddExpenseWindow:Window
{
 readonly ExpenseService service=new();
 public AddExpenseWindow(){InitializeComponent();MethodBox.SelectedIndex=0;}
 async void Save_Click(object s,RoutedEventArgs e){if(string.IsNullOrWhiteSpace(CategoryBox.Text)||!decimal.TryParse(AmountBox.Text,out var amount)||amount<=0){Status.Text="Category and valid amount are required.";return;}try{var method=(MethodBox.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString()??"Cash";await service.AddAsync(CategoryBox.Text,DescriptionBox.Text,amount,PaidToBox.Text,method,null);DialogResult=true;}catch(Exception ex){Status.Text=ex.Message;}}
}