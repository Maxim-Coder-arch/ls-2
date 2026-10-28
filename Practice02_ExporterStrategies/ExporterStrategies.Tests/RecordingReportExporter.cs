namespace ExporterStrategies.Tests;

internal sealed class RecordingReportExporter : IReportExporter
{
    private readonly List<ReportDocument> _exportedDocuments = new List<ReportDocument>();

    public IReadOnlyList<ReportDocument> ExportedDocuments => _exportedDocuments;

    public void Export(ReportDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);

        _exportedDocuments.Add(document);
    }
}
