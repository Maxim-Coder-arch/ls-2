namespace ExporterStrategies;

public sealed class TextFileReportExporter : IReportExporter
{
    private readonly string _targetFilePath;

    public TextFileReportExporter(string targetFilePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(targetFilePath);

        _targetFilePath = targetFilePath;
    }

    public void Export(ReportDocument document)
    {
        ReportDocumentValidator.EnsureIsExportable(document);

        string reportFileContent = BuildReportFileContent(document);

        File.WriteAllText(_targetFilePath, reportFileContent);
    }

    private static string BuildReportFileContent(ReportDocument document)
    {
        return $"{document.Title}{Environment.NewLine}{document.Body}";
    }
}
