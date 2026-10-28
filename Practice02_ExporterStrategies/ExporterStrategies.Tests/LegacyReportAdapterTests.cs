namespace ExporterStrategies.Tests;

internal sealed class LegacyReportAdapterTests
{
    private const string LegacyReportEndMarker = "=== END OF REPORT ===";

    public void RegisterTo(TestRunner testRunner)
    {
        ArgumentNullException.ThrowIfNull(testRunner);

        testRunner.Register(
            "legacy-адаптер выводит сводку в консоль через ReportService",
            AdapterPrintsDailySummaryToConsole);

        testRunner.Register(
            "legacy-адаптер сохраняет формат вывода принтера",
            AdapterKeepsLegacyOutputFormat);

        testRunner.Register(
            "legacy-адаптер не принимает null-принтер",
            ConstructorRejectsNullLegacyPrinter);
    }

    private static string PublishDailySummaryAndCaptureConsoleOutput()
    {
        LegacyReportAdapter legacyReportAdapter = new LegacyReportAdapter(new LegacyReportPrinter());
        ReportService reportService = new ReportService(legacyReportAdapter);

        return ConsoleOutputCapture.Capture(
            () => reportService.PublishDailySummary(totalJobs: 10, failedJobs: 1));
    }

    private static void AdapterPrintsDailySummaryToConsole()
    {
        string capturedConsoleOutput = PublishDailySummaryAndCaptureConsoleOutput();

        Assertion.Contains("=== Сводка за день ===", capturedConsoleOutput);
        Assertion.Contains("Всего задач: 10", capturedConsoleOutput);
        Assertion.Contains("Ошибок: 1", capturedConsoleOutput);
    }

    private static void AdapterKeepsLegacyOutputFormat()
    {
        string capturedConsoleOutput = PublishDailySummaryAndCaptureConsoleOutput();

        string expectedConsoleOutput = $"=== Сводка за день ==={Environment.NewLine}"
            + $"Всего задач: 10\nОшибок: 1{Environment.NewLine}"
            + $"{LegacyReportEndMarker}{Environment.NewLine}";

        Assertion.AreEqual(expectedConsoleOutput, capturedConsoleOutput);
    }

    private static void ConstructorRejectsNullLegacyPrinter()
    {
        ArgumentNullException thrownException = Assertion.Throws<ArgumentNullException>(
            () => new LegacyReportAdapter(null!));

        Assertion.AreEqual("legacyReportPrinter", thrownException.ParamName);
    }
}
