namespace LegacyReportAdapter;

public sealed class LegacyReportAdapter : IReportExporter
{
    private readonly LegacyReportPrinter _legacyReportPrinter;

    public LegacyReportAdapter(LegacyReportPrinter legacyReportPrinter)
    {
        ArgumentNullException.ThrowIfNull(legacyReportPrinter);

        _legacyReportPrinter = legacyReportPrinter;
    }

    public void Export(ReportDocument document)
    {
        ReportDocumentValidator.EnsureIsExportable(document);

        string legacyReportTitle = document.Title;
        string legacyReportText = document.Body;

        _legacyReportPrinter.PrintReport(legacyReportTitle, legacyReportText);
    }
}
