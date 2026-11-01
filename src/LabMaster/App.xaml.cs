using System.Windows;
using LabMaster.Services;

namespace LabMaster;

public partial class App : Application
{
 protected override void OnStartup(StartupEventArgs e)
 {
  base.OnStartup(e);

  // Keep the application alive while the login dialog is shown.
  ShutdownMode = ShutdownMode.OnExplicitShutdown;

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
  ShutdownMode = ShutdownMode.OnMainWindowClose;
  main.Show();
 }
}
