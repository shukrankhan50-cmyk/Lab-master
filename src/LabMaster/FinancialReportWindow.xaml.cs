using System.Windows;
using LabMaster.Services;

namespace LabMaster;

public partial class FinancialReportWindow : Window
{
    readonly FinancialReportService service = new();

    public FinancialReportWindow()
    {
        InitializeComponent();
        FromBox.SelectedDate = DateTime.Today;
        ToBox.SelectedDate = DateTime.Today;
    }

    async void Refresh_Click(object s, RoutedEventArgs e)
    {
        var from = FromBox.SelectedDate ?? DateTime.Today;
        var to = ToBox.SelectedDate ?? DateTime.Today;

        if (from.Date > to.Date)
        {
            MessageBox.Show(
                "From date cannot be later than To date.",
                "Lab Master",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        try
        {
            var x = await service.GetAsync(from, to);
            CollectionText.Text = $"Collection: PKR {x.Collection:N2}";
            ExpenseText.Text = $"Expenses: PKR {x.Expenses:N2}";
            BilledText.Text = $"Billed: PKR {x.Billed:N2}";\n            OrdersText.Text = $"Orders: {x.Orders:N0}";\n            NetText.Text = $"Net: PKR {x.Net:N2}";
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                "Lab Master",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}