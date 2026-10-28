namespace ExporterStrategies.Tests;

internal sealed class ReportServiceTests
{
    public void RegisterTo(TestRunner testRunner)
    {
        ArgumentNullException.ThrowIfNull(testRunner);

        testRunner.Register(
            "сервис отчётов не принимает null-экспортёр",
            ConstructorRejectsNullExporter);

        testRunner.Register(
            "сервис отчётов передаёт экспортёру документ со сводкой за день",
            PublishDailySummaryPassesSummaryDocumentToExporter);

        testRunner.Register(
            "сервис отчётов отклоняет отрицательное число задач",
            PublishDailySummaryRejectsNegativeTotalJobs);

        testRunner.Register(
            "сервис отчётов отклоняет отрицательное число ошибок",
            PublishDailySummaryRejectsNegativeFailedJobs);

        testRunner.Register(
            "сервис отчётов отклоняет ошибок больше, чем задач",
            PublishDailySummaryRejectsFailedJobsGreaterThanTotalJobs);

        testRunner.Register(
            "сервис отчётов работает с любым экспортёром без изменения кода",
            ServiceWorksWithEveryExporterWithoutChanges);
    }

    private static void ConstructorRejectsNullExporter()
    {
        ArgumentNullException thrownException = Assertion.Throws<ArgumentNullException>(
            () => new ReportService(null!));

        Assertion.AreEqual("reportExporter", thrownException.ParamName);
    }

    private static void PublishDailySummaryPassesSummaryDocumentToExporter()
    {
        RecordingReportExporter recordingReportExporter = new RecordingReportExporter();
        ReportService reportService = new ReportService(recordingReportExporter);

        reportService.PublishDailySummary(totalJobs: 10, failedJobs: 1);

        Assertion.AreEqual(1, recordingReportExporter.ExportedDocuments.Count);

        ReportDocument exportedDocument = recordingReportExporter.ExportedDocuments[0];

        Assertion.AreEqual("Сводка за день", exportedDocument.Title);
        Assertion.AreEqual("Всего задач: 10\nОшибок: 1", exportedDocument.Body);
    }

    private static void PublishDailySummaryRejectsNegativeTotalJobs()
    {
        RecordingReportExporter recordingReportExporter = new RecordingReportExporter();
        ReportService reportService = new ReportService(recordingReportExporter);

        Assertion.Throws<ArgumentOutOfRangeException>(
            () => reportService.PublishDailySummary(totalJobs: -1, failedJobs: 0));

        Assertion.AreEqual(0, recordingReportExporter.ExportedDocuments.Count);
    }

    private static void PublishDailySummaryRejectsNegativeFailedJobs()
    {
        RecordingReportExporter recordingReportExporter = new RecordingReportExporter();
        ReportService reportService = new ReportService(recordingReportExporter);

        Assertion.Throws<ArgumentOutOfRangeException>(
            () => reportService.PublishDailySummary(totalJobs: 5, failedJobs: -1));

        Assertion.AreEqual(0, recordingReportExporter.ExportedDocuments.Count);
    }

    private static void PublishDailySummaryRejectsFailedJobsGreaterThanTotalJobs()
    {
        RecordingReportExporter recordingReportExporter = new RecordingReportExporter();
        ReportService reportService = new ReportService(recordingReportExporter);

        Assertion.Throws<ArgumentOutOfRangeException>(
            () => reportService.PublishDailySummary(totalJobs: 5, failedJobs: 6));

        Assertion.AreEqual(0, recordingReportExporter.ExportedDocuments.Count);
    }

    private static void ServiceWorksWithEveryExporterWithoutChanges()
    {
        using TemporaryReportFile temporaryReportFile = TemporaryReportFile.CreateInExistingDirectory();

        RecordingReportExporter recordingReportExporter = new RecordingReportExporter();
        TextFileReportExporter textFileReportExporter = new TextFileReportExporter(temporaryReportFile.ReportFilePath);
        LegacyReportAdapter legacyReportAdapter = new LegacyReportAdapter(new LegacyReportPrinter());

        IReportExporter[] reportExporters =
        {
            recordingReportExporter,
            textFileReportExporter,
            legacyReportAdapter
        };

        string capturedConsoleOutput = ConsoleOutputCapture.Capture(
            () => PublishDailySummaryThroughEveryExporter(reportExporters));

        Assertion.AreEqual(1, recordingReportExporter.ExportedDocuments.Count);
        Assertion.IsTrue(
            File.Exists(temporaryReportFile.ReportFilePath),
            "Text file exporter did not create the report file.");
        Assertion.Contains("=== Сводка за день ===", capturedConsoleOutput);
    }

    private static void PublishDailySummaryThroughEveryExporter(IEnumerable<IReportExporter> reportExporters)
    {
        foreach (IReportExporter reportExporter in reportExporters)
        {
            ReportService reportService = new ReportService(reportExporter);

            reportService.PublishDailySummary(totalJobs: 10, failedJobs: 1);
        }
    }
}
