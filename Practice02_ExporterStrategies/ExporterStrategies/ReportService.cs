namespace ExporterStrategies;

public sealed class ReportService
{
    private const string DailySummaryTitle = "Сводка за день";

    private readonly IReportExporter _reportExporter;

    public ReportService(IReportExporter reportExporter)
    {
        ArgumentNullException.ThrowIfNull(reportExporter);

        _reportExporter = reportExporter;
    }

    public void PublishDailySummary(int totalJobs, int failedJobs)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(totalJobs);
        ArgumentOutOfRangeException.ThrowIfNegative(failedJobs);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(failedJobs, totalJobs);

        string dailySummaryBody = BuildDailySummaryBody(totalJobs, failedJobs);
        ReportDocument dailySummaryDocument = new ReportDocument(DailySummaryTitle, dailySummaryBody);

        _reportExporter.Export(dailySummaryDocument);
    }

    private static string BuildDailySummaryBody(int totalJobs, int failedJobs)
    {
        return $"Всего задач: {totalJobs}\nОшибок: {failedJobs}";
    }
}
