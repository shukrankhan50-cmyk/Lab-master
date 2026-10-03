using System.Windows;
namespace LabMaster;
public partial class App : Application
{
 protected override void OnStartup(StartupEventArgs e)
 {
  base.OnStartup(e);
  var login=new LoginWindow();
  if(login.ShowDialog()!=true){Shutdown();return;}
  var main=new MainWindow();
  MainWindow=main;
  main.Show();
 }
}