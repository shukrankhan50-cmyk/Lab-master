using System.Windows;
namespace LabMaster;
public partial class MainWindow : Window
{
 public MainWindow(){InitializeComponent();}
 private void PatientRegistration_Click(object sender,RoutedEventArgs e){new PatientRegistrationWindow{Owner=this}.ShowDialog();}
 private void Billing_Click(object sender, RoutedEventArgs e){new BillingWindow{Owner=this}.ShowDialog();}\n private void Departments_Click(object sender, RoutedEventArgs e){new DepartmentWindow{Owner=this}.ShowDialog();}\n private void Backup_Click(object sender, RoutedEventArgs e){new BackupWindow{Owner=this}.ShowDialog();}\n private void TestMaster_Click(object sender, RoutedEventArgs e){new TestMasterWindow{Owner=this}.ShowDialog();}\n private void Reports_Click(object sender, RoutedEventArgs e){new ReportWindow{Owner=this}.ShowDialog();}\n private void ResultEntry_Click(object sender, RoutedEventArgs e){new ResultEntryWindow{Owner=this}.ShowDialog();}\n private void AcceptTest_Click(object sender, RoutedEventArgs e){new AcceptTestWindow{Owner=this}.ShowDialog();}\n private void PatientSearch_Click(object sender,RoutedEventArgs e){new PatientSearchWindow{Owner=this}.ShowDialog();}
}