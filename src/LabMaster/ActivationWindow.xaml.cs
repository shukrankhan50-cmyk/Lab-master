using System.Windows;
using LabMaster.Services;

namespace LabMaster;

public partial class ActivationWindow : Window
{
    private readonly LicenseService service = new();

    public ActivationWindow()
    {
        InitializeComponent();
    }

    private void Activate_Click(object sender, RoutedEventArgs e)
    {
        if (service.Activate(KeyBox.Text, out var message))
        {
            MessageBox.Show(message, "Lab Master", MessageBoxButton.OK, MessageBoxImage.Information);
            DialogResult = true;
            return;
        }

        StatusText.Text = message;
    }
}