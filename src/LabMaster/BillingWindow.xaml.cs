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
            ReceiptsGrid.ItemsSource = await service.GetReceiptsAsync();
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

    async void RefreshReceipts_Click(object sender, RoutedEventArgs e)
    {
        try { ReceiptsGrid.ItemsSource = await service.GetReceiptsAsync(); }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Lab Master", MessageBoxButton.OK, MessageBoxImage.Error); }
    }

    void PrintReceipt_Click(object sender, RoutedEventArgs e)
    {
        if (ReceiptsGrid.SelectedItem is not ReceiptRow receipt)
        {
            MessageBox.Show("Select a receipt first.");
            return;
        }

        var panel = new System.Windows.Controls.StackPanel
        {
            Width = 520,
            Margin = new Thickness(30)
        };
        panel.Children.Add(new System.Windows.Controls.TextBlock { Text = "LAB MASTER", FontSize = 24, FontWeight = FontWeights.Bold, HorizontalAlignment = HorizontalAlignment.Center });
        panel.Children.Add(new System.Windows.Controls.TextBlock { Text = "Payment Receipt", FontSize = 18, Margin = new Thickness(0, 8, 0, 20), HorizontalAlignment = HorizontalAlignment.Center });
        panel.Children.Add(new System.Windows.Controls.TextBlock { Text = $"Receipt: {receipt.ReceiptNumber}\nOrder: {receipt.OrderNumber}\nMR: {receipt.MRNumber}\nPatient: {receipt.PatientName}\nAmount: Rs. {receipt.Amount:N2}\nMethod: {receipt.PaymentMethod}\nDate: {receipt.PaidAt:dd-MMM-yyyy HH:mm}\nReceived By: {receipt.ReceivedBy ?? "-"}", FontSize = 15, Margin = new Thickness(0, 0, 0, 20) });
        panel.Children.Add(new System.Windows.Controls.TextBlock { Text = "Thank you.", FontSize = 13, HorizontalAlignment = HorizontalAlignment.Center });

        var dlg = new PrintDialog();
        if (dlg.ShowDialog() == true) dlg.PrintVisual(panel, $"Receipt - {receipt.ReceiptNumber}");
    }

    void Clear_Click(object sender, RoutedEventArgs e)
    {
        selected = null;
        BillsGrid.SelectedItem = null;
        AmountBox.Clear();
        BalanceLabel.Text = "";
    }
}