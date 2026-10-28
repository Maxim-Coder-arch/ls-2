namespace LegacyReportAdapter;

internal static class ReportDocumentValidator
{
    public static void EnsureIsExportable(ReportDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);

        if (string.IsNullOrWhiteSpace(document.Title))
        {
            throw new ArgumentException(
                "Report title must not be null, empty or whitespace.",
                nameof(document));
        }

        if (document.Body is null)
        {
            throw new ArgumentException(
                "Report body must not be null. Use an empty string for a report without a body.",
                nameof(document));
        }
    }
}
