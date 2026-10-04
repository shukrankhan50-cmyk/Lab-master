using System.Windows;
using LabMaster.Models;
using LabMaster.Services;

namespace LabMaster;

public partial class TestMasterWindow : Window
{
    readonly TestMasterService service = new();

    public TestMasterWindow()
    {
        InitializeComponent();
        Loaded += async (_, _) => await LoadAsync("");
    }

    async Task LoadAsync(string q)
    {
        try { TestsGrid.ItemsSource = await service.GetAsync(q); }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Lab Master", MessageBoxButton.OK, MessageBoxImage.Error); }
    }

    async void Search_TextChanged(object s, System.Windows.Controls.TextChangedEventArgs e)
    {
        if (IsLoaded) await LoadAsync(SearchBox.Text);
    }

    async void Add_Click(object s, RoutedEventArgs e)
    {
        var w = new AddTestWindow { Owner = this };
        if (w.ShowDialog() == true) await LoadAsync(SearchBox.Text);
    }

    async void Edit_Click(object s, RoutedEventArgs e)
    {
        if (TestsGrid.SelectedItem is not TestMasterItem t) { MessageBox.Show("Select a test."); return; }
        var w = new EditTestWindow(t) { Owner = this };
        if (w.ShowDialog() == true) await LoadAsync(SearchBox.Text);
    }

    async void Toggle_Click(object s, RoutedEventArgs e)
    {
        if (TestsGrid.SelectedItem is not TestMasterItem t) { MessageBox.Show("Select a test."); return; }
        try { await service.SetActiveAsync(t.TestId, !t.IsActive); await LoadAsync(SearchBox.Text); }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Lab Master", MessageBoxButton.OK, MessageBoxImage.Error); }
    }
}