using System.Windows;
using System.Windows.Controls;
using LabMaster.Services;

namespace LabMaster;

public partial class AcceptTestWindow : Window
{
    private readonly TestOrderService service = new();
    private int? patientId;
    private bool isLoadingTests;
    private CancellationTokenSource? searchDelay;

    public AcceptTestWindow()
    {
        InitializeComponent();
        Loaded += async (_, _) => await LoadTestsAsync();
    }

    private async Task LoadTestsAsync()
    {
        isLoadingTests = true;
        UpdateCreateOrderState();
        try
        {
            TestsGrid.ItemsSource = await service.GetTestsAsync();
            StatusLabel.Text = "Find a patient, then select one or more tests.";
        }
        catch
        {
            StatusLabel.Text = "Tests could not be loaded. Check the SQL Server connection.";
        }
        finally
        {
            isLoadingTests = false;
            UpdateSelectionSummary();
        }
    }

    private async void Search_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (!IsLoaded || isLoadingTests) return;

        searchDelay?.Cancel();
        var currentSearch = new CancellationTokenSource();
        searchDelay = currentSearch;

        try
        {
            await Task.Delay(300, currentSearch.Token);
            var tests = await service.GetTestsAsync(TestSearchBox.Text);
            if (currentSearch.IsCancellationRequested) return;

            TestsGrid.ItemsSource = tests;
            StatusLabel.Text = tests.Count == 0
                ? "No active tests match that search."
                : "Select one or more tests. Their total appears on the right.";
        }
        catch (OperationCanceledException)
        {
        }
        catch
        {
            if (!currentSearch.IsCancellationRequested)
                StatusLabel.Text = "Test search failed. Check the SQL Server connection and try again.";
        }
        finally
        {
            if (ReferenceEquals(searchDelay, currentSearch))
                searchDelay = null;
            currentSearch.Dispose();
        }
    }

    private async void FindPatient_Click(object sender, RoutedEventArgs e)
    {
        var mrNumber = MrBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(mrNumber))
        {
            StatusLabel.Text = "Enter the patient's MR number first.";
            MrBox.Focus();
            return;
        }

        patientId = null;
        PatientLabel.Text = "Searching for patient...";
        UpdateCreateOrderState();

        try
        {
            var search = new PatientSearchService();
            var matches = await search.SearchAsync(mrNumber);
            var match = matches.Find(x =>
                x.MRNumber.Equals(mrNumber, StringComparison.OrdinalIgnoreCase));

            if (match is null)
            {
                PatientLabel.Text = "No patient found.";
                StatusLabel.Text = "Check the MR number and search again.";
                return;
            }

            patientId = match.PatientId;
            PatientLabel.Text = $"{match.PatientName}  •  {match.Gender}  •  {match.Phone ?? "No phone on file"}";
            StatusLabel.Text = "Patient selected. Choose the tests for this order.";
        }
        catch
        {
            PatientLabel.Text = "Patient details unavailable.";
            StatusLabel.Text = "Patient search failed. Check the SQL Server connection and try again.";
        }
        finally
        {
            UpdateCreateOrderState();
        }
    }

    private void MrBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (patientId is null) return;

        patientId = null;
        PatientLabel.Text = "Search again to confirm this MR number.";
        StatusLabel.Text = "The MR number changed. Find the patient again before creating an order.";
        UpdateCreateOrderState();
    }

    private void TestsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) =>
        UpdateSelectionSummary();

    private void UpdateSelectionSummary()
    {
        var selected = TestsGrid.SelectedItems.OfType<TestOption>().ToList();
        SelectedLabel.Text = selected.Count == 0
            ? "No tests selected."
            : string.Join(Environment.NewLine, selected.Select(x => x.TestName));
        TotalLabel.Text = $"{selected.Count} {(selected.Count == 1 ? "test" : "tests")}  •  Rs. {selected.Sum(x => x.Price):N0}";
        UpdateCreateOrderState();
    }

    private void UpdateCreateOrderState() =>
        CreateOrderButton.IsEnabled = patientId.HasValue
            && TestsGrid.SelectedItems.Count > 0
            && !isLoadingTests;

    private async void CreateOrder_Click(object sender, RoutedEventArgs e)
    {
        if (patientId is not int selectedPatientId)
        {
            StatusLabel.Text = "Find a patient before creating an order.";
            return;
        }

        var selectedTests = TestsGrid.SelectedItems.OfType<TestOption>().ToList();
        if (selectedTests.Count == 0)
        {
            StatusLabel.Text = "Select at least one test before creating an order.";
            return;
        }

        CreateOrderButton.IsEnabled = false;
        try
        {
            var orderNumber = await service.CreateOrderAsync(
                selectedPatientId,
                selectedTests.Select(x => x.TestId));
            MessageBox.Show($"Order created successfully.\nOrder Number: {orderNumber}", "Lab Master",
                MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }
        catch (Exception ex)
        {
            StatusLabel.Text = "The order could not be created. Check the message, then try again.";
            MessageBox.Show(ex.Message, "Lab Master", MessageBoxButton.OK, MessageBoxImage.Error);
            UpdateCreateOrderState();
        }
    }
}