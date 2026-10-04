using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using LabMaster.Services;

namespace LabMaster;

public partial class ReportWindow : Window
{
    readonly ReportService service = new();
    readonly ReportSettingsService settingsService = new();
    ReportOrder? selected;
    ReportData? report;

    public ReportWindow()
    {
        InitializeComponent();
        Loaded += async (_, _) => await LoadAsync();
    }

    async Task LoadAsync()
    {
        try
        {
            OrdersGrid.ItemsSource = await service.GetEnteredOrdersAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Lab Master", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    async void OrdersGrid_SelectionChanged(object s, SelectionChangedEventArgs e)
    {
        try
        {
            selected = OrdersGrid.SelectedItem as ReportOrder;
            if (selected == null) return;

            await service.EnsureReportNumberAsync(selected.OrderId);
            report = await service.GetReportAsync(selected.OrderId);
            if (report == null) return;

            var settings = await settingsService.GetAsync();
            LabNameText.Text = settings.LaboratoryName;
            LabAddressText.Text = settings.Address ?? "";
            LabPhoneText.Text = settings.Phone ?? "";
            FooterText.Text = settings.ReportFooter ?? "";

            PatientInfo.Text =
                $"Patient: {report.PatientName}\n" +
                $"MR: {report.MRNumber}    Gender: {report.Gender}" +
                (string.IsNullOrWhiteSpace(report.Phone) ? "" : $"    Phone: {report.Phone}");

            ReportInfo.Text =
                $"Report: {report.ReportNumber}\n" +
                $"Order: {report.OrderNumber}\n" +
                $"Date: {report.OrderDate:dd-MMM-yyyy HH:mm}";

            ReportGrid.ItemsSource = report.Results;

            CommentText.Text = string.Join(
                Environment.NewLine,
                report.Results
                    .Where(x => !string.IsNullOrWhiteSpace(x.Comment))
                    .Select(x => x.Comment));

            VerifiedText.Text = report.VerifiedBy == null
                ? "Pending Verification"
                : $"{report.VerifiedBy}  |  {report.VerifiedAt:dd-MMM-yyyy HH:mm}";

            StatusText.Text = report.VerifiedBy == null ? "PENDING" : "VERIFIED";
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Lab Master", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    async void Verify_Click(object s, RoutedEventArgs e)
    {
        if (selected == null)
        {
            MessageBox.Show("Select a report first.", "Lab Master", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            await service.VerifyAsync(selected.OrderId, CurrentUserContext.UserName);
            report = await service.GetReportAsync(selected.OrderId);
            if (report != null)
            {
                VerifiedText.Text = $"{report.VerifiedBy}  |  {report.VerifiedAt:dd-MMM-yyyy HH:mm}";
                StatusText.Text = "VERIFIED";
            }

            MessageBox.Show("Report verified successfully.", "Lab Master", MessageBoxButton.OK, MessageBoxImage.Information);
            await LoadAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Lab Master", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    void Print_Click(object s, RoutedEventArgs e)
    {
        if (report == null)
        {
            MessageBox.Show("Select a report first.", "Lab Master", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (report.VerifiedBy == null)
        {
            MessageBox.Show(
                "This report has not been verified yet. Verify the report before printing.",
                "Lab Master",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        var dlg = new PrintDialog();
        if (dlg.ShowDialog() != true) return;

        var document = BuildPrintableDocument();
        document.PageWidth = dlg.PrintableAreaWidth;
        document.PageHeight = dlg.PrintableAreaHeight;
        document.ColumnWidth = dlg.PrintableAreaWidth;
        document.PagePadding = new Thickness(42);

        dlg.PrintDocument(
            ((IDocumentPaginatorSource)document).DocumentPaginator,
            $"Lab Report - {report.ReportNumber ?? report.MRNumber}");
    }

    FlowDocument BuildPrintableDocument()
    {
        var document = new FlowDocument
        {
            FontFamily = new System.Windows.Media.FontFamily("Arial"),
            FontSize = 10
        };

        var header = new Paragraph
        {
            TextAlignment = TextAlignment.Center,
            Margin = new Thickness(0, 0, 0, 12)
        };
        header.Inlines.Add(new Run(LabNameText.Text)
        {
            FontSize = 18,
            FontWeight = FontWeights.Bold
        });
        header.Inlines.Add(new LineBreak());
        header.Inlines.Add(new Run(LabAddressText.Text)
        {
            FontSize = 9
        });
        if (!string.IsNullOrWhiteSpace(LabPhoneText.Text))
        {
            header.Inlines.Add(new LineBreak());
            header.Inlines.Add(new Run(LabPhoneText.Text) { FontSize = 9 });
        }
        document.Blocks.Add(header);

        var info = new Table
        {
            CellSpacing = 0,
            BorderBrush = System.Windows.Media.Brushes.LightGray,
            BorderThickness = new Thickness(0, 1, 0, 1),
            Margin = new Thickness(0, 0, 0, 12)
        };
        info.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });
        info.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });

        var infoRow = new TableRow();
        var patientCell = new TableCell(new Paragraph(new Run(
            $"PATIENT INFORMATION\\nPatient: {report!.PatientName}\\nMR: {report.MRNumber}    Gender: {report.Gender}" +
            (string.IsNullOrWhiteSpace(report.Phone) ? "" : $"    Phone: {report.Phone}"))))
        {
            Padding = new Thickness(6)
        };
        var reportCell = new TableCell(new Paragraph(new Run(
            $"REPORT INFORMATION\\nReport: {report.ReportNumber}\\nOrder: {report.OrderNumber}\\nDate: {report.OrderDate:dd-MMM-yyyy HH:mm}")))
        {
            Padding = new Thickness(6)
        };
        reportCell.TextAlignment = TextAlignment.Right;
        infoRow.Cells.Add(patientCell);
        infoRow.Cells.Add(reportCell);
        var infoGroup = new TableRowGroup();
        infoGroup.Rows.Add(infoRow);
        info.RowGroups.Add(infoGroup);
        document.Blocks.Add(info);

        document.Blocks.Add(new Paragraph(new Run("LABORATORY RESULTS"))
        {
            FontWeight = FontWeights.Bold,
            FontSize = 12,
            Margin = new Thickness(0, 0, 0, 6)
        });

        var results = new Table
        {
            CellSpacing = 0,
            BorderBrush = System.Windows.Media.Brushes.Gray,
            BorderThickness = new Thickness(1),
            RepeatHeader = true
        };
        results.Columns.Add(new TableColumn { Width = new GridLength(2.2, GridUnitType.Star) });
        results.Columns.Add(new TableColumn { Width = new GridLength(1.1, GridUnitType.Star) });
        results.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });
        results.Columns.Add(new TableColumn { Width = new GridLength(1.6, GridUnitType.Star) });

        var headerGroup = new TableRowGroup();
        var headerRow = new TableRow();
        foreach (var title in new[] { "Test", "Result", "Unit", "Reference Range" })
        {
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run(title)))
            {
                FontWeight = FontWeights.Bold,
                Background = System.Windows.Media.Brushes.LightGray,
                Padding = new Thickness(5)
            });
        }
        headerGroup.Rows.Add(headerRow);
        results.RowGroups.Add(headerGroup);

        var resultGroup = new TableRowGroup();
        foreach (var item in report.Results)
        {
            var row = new TableRow();
            foreach (var value in new[]
            {
                item.TestName,
                item.ResultValue ?? "",
                item.Unit ?? "",
                item.ReferenceRange ?? ""
            })
            {
                row.Cells.Add(new TableCell(new Paragraph(new Run(value)))
                {
                    Padding = new Thickness(5)
                });
            }
            resultGroup.Rows.Add(row);
        }
        results.RowGroups.Add(resultGroup);
        document.Blocks.Add(results);

        var comments = report.Results
            .Where(x => !string.IsNullOrWhiteSpace(x.Comment))
            .Select(x => x.Comment!)
            .ToList();

        if (comments.Count > 0)
        {
            document.Blocks.Add(new Paragraph(new Run("COMMENTS / INTERPRETATION"))
            {
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 14, 0, 4)
            });
            document.Blocks.Add(new Paragraph(new Run(string.Join(Environment.NewLine, comments))));
        }

        var sign = new Table
        {
            CellSpacing = 0,
            Margin = new Thickness(0, 24, 0, 0)
        };
        sign.Columns.Add(new TableColumn());
        sign.Columns.Add(new TableColumn());
        sign.Columns.Add(new TableColumn());

        var signRow = new TableRow();
        foreach (var text in new[]
        {
            "Laboratory Technician\\n________________",
            $"Verified By\\n{report.VerifiedBy}",
            "Report Status\\nVERIFIED"
        })
        {
            signRow.Cells.Add(new TableCell(new Paragraph(new Run(text)))
            {
                Padding = new Thickness(4)
            });
        }
        var signGroup = new TableRowGroup();
        signGroup.Rows.Add(signRow);
        sign.RowGroups.Add(signGroup);
        document.Blocks.Add(sign);

        document.Blocks.Add(new Paragraph(new Run(FooterText.Text))
        {
            FontSize = 8,
            Foreground = System.Windows.Media.Brushes.Gray,
            TextAlignment = TextAlignment.Center,
            Margin = new Thickness(0, 18, 0, 0)
        });

        return document;
    }
}
