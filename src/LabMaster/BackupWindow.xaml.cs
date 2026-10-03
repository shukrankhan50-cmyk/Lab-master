using System.Windows;
using Microsoft.Win32;
using LabMaster.Services;
namespace LabMaster;
public partial class BackupWindow:Window
{
 readonly BackupService service=new();
 public BackupWindow(){InitializeComponent();}
 async void Backup_Click(object s,RoutedEventArgs e){var dlg=new SaveFileDialog{Filter="SQL Backup (*.bak)|*.bak",FileName="LabMasterBackup-"+DateTime.Now.ToString("yyyyMMdd-HHmmss")+".bak"};if(dlg.ShowDialog()!=true)return;try{await service.BackupAsync(dlg.FileName);Status.Text="Backup created successfully."; }catch(Exception ex){Status.Text="Backup failed: "+ex.Message;}}
}