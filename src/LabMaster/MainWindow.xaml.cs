using System.Windows;

namespace LabMaster;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void PatientRegistration_Click(object sender, RoutedEventArgs e)
    {
        var window = new PatientRegistrationWindow
        {
            Owner = this
        };
        window.ShowDialog();
    }
}