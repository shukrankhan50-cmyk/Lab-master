using System.Windows;
using LabMaster.Services;

namespace LabMaster;

public partial class AnalyzerMaintenanceWindow : Window
{
    readonly AnalyzerService service = new();
    readonly int analyzerId;
    readonly string analyzerName;

    public AnalyzerMaintenanceWindow(int id, string name)
    {
        InitializeComponent();
        analyzerId = id;
        analyzerName = name;
        TitleText.Text = $"{name} — Maintenance History";
        Loaded += async (_, _) => await LoadAsync();
    }

    async Task LoadAsync()
    {
        try
        {
            var rows = await service.GetMaintenanceAsync(analyzerId);
            Grid.ItemsSource = rows;
            InfoText.Text = rows.Count == 0
                ? "No maintenance or calibration records yet."
                : $"{rows.Count} record(s) • Latest activity shown first.";
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Lab Master", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    async void Add_Click(object sender, RoutedEventArgs e)
    {
        var w = new AddAnalyzerMaintenanceWindow(analyzerId, analyzerName) { Owner = this };
        if (w.ShowDialog() == true)
            await LoadAsync();
    }

    void Close_Click(object sender, RoutedEventArgs e) => Close();
}