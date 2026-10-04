using System.Windows;
using LabMaster.Services;

namespace LabMaster;

public partial class App : Application
{
 protected override void OnStartup(StartupEventArgs e)
 {
  base.OnStartup(e);

  var license = new LicenseService().GetStatus();
  if (!license.IsActive)
  {
   var activation = new ActivationWindow();
   if (activation.ShowDialog() != true)
   {
    Shutdown();
    return;
   }
  }

  var login = new LoginWindow();
  if (login.ShowDialog() != true)
  {
   Shutdown();
   return;
  }

  if (login.LoggedInUser is null) { Shutdown(); return; }
  CurrentUserContext.Set(login.LoggedInUser);

  var main = new MainWindow();
  MainWindow = main;
  main.Show();
 }
}