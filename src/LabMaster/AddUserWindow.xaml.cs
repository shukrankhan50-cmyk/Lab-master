using System.Windows;
using LabMaster.Services;
namespace LabMaster;
public partial class AddUserWindow:Window
{
 readonly UserService service=new();
 public AddUserWindow(){InitializeComponent();RoleBox.SelectedIndex=2;}
 async void Save_Click(object s,RoutedEventArgs e){if(string.IsNullOrWhiteSpace(UserBox.Text)||string.IsNullOrWhiteSpace(DisplayBox.Text)||PasswordBox.Password.Length<6){StatusText.Text="Username, display name and password (6+ chars) are required.";return;}try{var role=(RoleBox.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString()??"Technician";await service.AddAsync(UserBox.Text,DisplayBox.Text,PasswordBox.Password,role);DialogResult=true;}catch(Exception ex){StatusText.Text=ex.Message;}}
}