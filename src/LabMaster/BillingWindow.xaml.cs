using System.Windows;
using LabMaster.Services;

namespace LabMaster;

public partial class BillingWindow : Window
{
    readonly BillingService service = new();
    BillItem? selected;

    public BillingWindow()
    {
        InitializeComponent();
        Loaded += async (_, _) => await LoadAsync();
    }

    async Task LoadAsync()
    {
        try
        {
            BillsGrid.ItemsSource = await service.GetPendingBillsAsync();
            BalanceLabel.Text = "";
            selected = null;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Lab Master", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    void BillsGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        selected = BillsGrid.SelectedItem as BillItem;
        if (selected == null) return;

        BalanceLabel.Text = $"Balance: Rs. {selected.Balance:N2}";
        AmountBox.Text = selected.Balance.ToString("0.00");
        AmountBox.Focus();
        AmountBox.SelectAll();
    }

    async void Receive_Click(object sender, RoutedEventArgs e)
    {
        if (selected == null)
        {
            MessageBox.Show("Select an order first.");
            return;
        }

        if (!decimal.TryParse(AmountBox.Text, out var amount) || amount <= 0)
        {
            MessageBox.Show("Enter a valid payment amount.");
            return;
        }

        if (amount > selected.Balance)
        {
            MessageBox.Show("Payment cannot be greater than the remaining balance.");
            return;
        }

        if (MethodBox.SelectedItem is not System.Windows.Controls.ComboBoxItem method)
        {
            MessageBox.Show("Select a payment method.");
            return;
        }

        var paymentMethod = method.Content?.ToString() ?? "Cash";

        try
        {
            var no = await service.ReceiveAsync(
                selected.OrderId,
                amount,
                paymentMethod,
                CurrentUserContext.UserName);

            MessageBox.Show(
                $"Payment received successfully.\nReceipt: {no}",
                "Lab Master",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            await LoadAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Lab Master", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    void Clear_Click(object sender, RoutedEventArgs e)
    {
        selected = null;
        BillsGrid.SelectedItem = null;
        AmountBox.Clear();
        BalanceLabel.Text = "";
    }
}