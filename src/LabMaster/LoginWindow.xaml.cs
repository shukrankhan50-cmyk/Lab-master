using System.Windows;
using LabMaster.Models;
using LabMaster.Services;
namespace LabMaster;
public partial class LoginWindow:Window
{
 readonly AuthenticationService service=new();
 public CurrentUser? LoggedInUser{get;private set;}
 public LoginWindow(){InitializeComponent();UserBox.Text="admin";PasswordBox.Focus();}
 async void Login_Click(object s,RoutedEventArgs e)
 {
  try{var user=await service.LoginAsync(UserBox.Text,PasswordBox.Password);if(user==null){StatusText.Text="Invalid username or password.";return;}LoggedInUser=user; await new AuditService().WriteAsync(user.UserName,"LOGIN","User",user.UserId,"Successful login"); DialogResult=true;}catch(Exception ex){StatusText.Text=ex.Message;}
 }
}