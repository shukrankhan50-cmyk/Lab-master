using System.Windows;
using LabMaster.Services;

namespace LabMaster;

public partial class ChangePasswordWindow : Window
{
    readonly UserService service = new();
    readonly int userId;
    readonly string userName;

    public ChangePasswordWindow(UserRow u)
    {
        InitializeComponent();
        userId = u.UserId;
        userName = u.UserName;
        UserLabel.Text = $"User: {u.UserName}";
    }

    public ChangePasswordWindow(CurrentUser u)
        : this(new UserRow(u.UserId, u.UserName, u.DisplayName, u.RoleName, true))
    {
        UserLabel.Text = $"First login — create a new password for {u.UserName}";
    }

    async void Save_Click(object s, RoutedEventArgs e)
    {
        if (PasswordBox.Password.Length < 8)
        {
            StatusText.Text = "Password must be at least 8 characters.";
            return;
        }

        if (PasswordBox.Password != ConfirmBox.Password)
        {
            StatusText.Text = "Passwords do not match.";
            return;
        }

        try
        {
            await service.ChangePasswordAsync(userId, PasswordBox.Password);
            MessageBox.Show("Password changed successfully.", "Lab Master", MessageBoxButton.OK, MessageBoxImage.Information);
            DialogResult = true;
        }
        catch (Exception ex)
        {
            StatusText.Text = ex.Message;
        }
    }
}
