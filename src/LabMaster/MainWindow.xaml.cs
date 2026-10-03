using System.Windows;
namespace LabMaster;
public partial class MainWindow : Window
{
 public MainWindow(){InitializeComponent();}
 private void PatientRegistration_Click(object sender,RoutedEventArgs e){new PatientRegistrationWindow{Owner=this}.ShowDialog();}
 private void ResultEntry_Click(object sender, RoutedEventArgs e){new ResultEntryWindow{Owner=this}.ShowDialog();}\n private void AcceptTest_Click(object sender, RoutedEventArgs e){new AcceptTestWindow{Owner=this}.ShowDialog();}\n private void PatientSearch_Click(object sender,RoutedEventArgs e){new PatientSearchWindow{Owner=this}.ShowDialog();}
}