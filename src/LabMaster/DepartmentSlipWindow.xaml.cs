using System.Windows;
using System.Windows.Controls;
using LabMaster.Services;

namespace LabMaster;

public partial class DepartmentSlipWindow : Window
{
    readonly DepartmentSlipService service = new();
    SlipOrder? selected;

    public DepartmentSlipWindow()
    {
        InitializeComponent();
        Loaded += async (_, _) => await LoadAsync();
    }

    async Task LoadAsync()
    {
        try
        {
            OrdersGrid.ItemsSource = await service.GetOrdersAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Lab Master", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    async void Order_SelectionChanged(object s, SelectionChangedEventArgs e)
    {
        try
        {
            selected = OrdersGrid.SelectedItem as SlipOrder;
            if (selected == null) return;

            var items = await service.GetItemsAsync(selected.OrderId);
            PatientText.Text =
                $"Patient: {selected.PatientName}    MR: {selected.MRNumber}\n" +
                $"Order: {selected.OrderNumber}    Date: {selected.OrderDate:dd-MMM-yyyy HH:mm}";

            ItemsGrid.ItemsSource = items;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Lab Master", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    void Print_Click(object s, RoutedEventArgs e)
    {
        if (selected == null)
        {
            MessageBox.Show("Select an order first.", "Lab Master", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var d = new PrintDialog();
        if (d.ShowDialog() == true)
            d.PrintVisual(SlipPanel, $"Department Slip - {selected.OrderNumber}");
    }
}
