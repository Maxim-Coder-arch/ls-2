namespace LegacyReportAdapter.Tests;

internal sealed class LegacyReportAdapterTests
{
    private const string LegacyReportEndMarker = "=== END OF REPORT ===";

    public void RegisterTo(TestRunner testRunner)
    {
        ArgumentNullException.ThrowIfNull(testRunner);

        testRunner.Register(
            "заголовок передан legacy-принтеру",
            ExportPassesTitleToLegacyPrinter);

        testRunner.Register(
            "тело передано legacy-принтеру",
            ExportPassesBodyToLegacyPrinter);

        testRunner.Register(
            "сохранён завершающий маркер",
            ExportKeepsLegacyEndMarker);

        testRunner.Register(
            "формат вывода legacy-принтера не изменён",
            ExportKeepsExactLegacyOutputFormat);

        testRunner.Register(
            "многострочное тело передаётся без изменений",
            ExportKeepsMultilineBodyUnchanged);

        testRunner.Register(
            "пустое тело допустимо и печатается как пустая строка",
            ExportAllowsEmptyBody);

        testRunner.Register(
            "пустой заголовок отклоняется до печати",
            ExportRejectsEmptyTitleBeforePrinting);

        testRunner.Register(
            "заголовок из пробелов отклоняется до печати",
            ExportRejectsWhitespaceTitleBeforePrinting);

        testRunner.Register(
            "null-тело отклоняется до печати",
            ExportRejectsNullBodyBeforePrinting);

        testRunner.Register(
            "null-документ вызывает ArgumentNullException",
            ExportRejectsNullDocument);

        testRunner.Register(
            "конструктор не принимает null-зависимость",
            ConstructorRejectsNullLegacyPrinter);
    }

    private static string ExportAndCaptureConsoleOutput(ReportDocument document)
    {
        LegacyReportAdapter reportAdapter = new LegacyReportAdapter(new LegacyReportPrinter());

        return ConsoleOutputCapture.Capture(() => reportAdapter.Export(document));
    }

    private static string CreateExpectedLegacyOutput(string title, string body)
    {
        return $"=== {title} ==={Environment.NewLine}"
            + $"{body}{Environment.NewLine}"
            + $"{LegacyReportEndMarker}{Environment.NewLine}";
    }

    private static void ExportPassesTitleToLegacyPrinter()
    {
        ReportDocument document = new ReportDocument("Test title", "Test body");

        string capturedOutput = ExportAndCaptureConsoleOutput(document);

        Assertion.Contains("=== Test title ===", capturedOutput);
    }

    private static void ExportPassesBodyToLegacyPrinter()
    {
        ReportDocument document = new ReportDocument("Test title", "Test body");

        string capturedOutput = ExportAndCaptureConsoleOutput(document);

        Assertion.Contains("Test body", capturedOutput);
    }

    private static void ExportKeepsLegacyEndMarker()
    {
        ReportDocument document = new ReportDocument("Test title", "Test body");

        string capturedOutput = ExportAndCaptureConsoleOutput(document);

        Assertion.Contains(LegacyReportEndMarker, capturedOutput);
    }

    private static void ExportKeepsExactLegacyOutputFormat()
    {
        ReportDocument document = new ReportDocument("Test title", "Test body");

        string capturedOutput = ExportAndCaptureConsoleOutput(document);

        Assertion.AreEqual(CreateExpectedLegacyOutput("Test title", "Test body"), capturedOutput);
    }

    private static void ExportKeepsMultilineBodyUnchanged()
    {
        const string multilineBody = "Всего позиций: 18\nВ ремонте: 3\nСписано: 1";
        ReportDocument document = new ReportDocument("Состояние оборудования", multilineBody);

        string capturedOutput = ExportAndCaptureConsoleOutput(document);

        Assertion.AreEqual(
            CreateExpectedLegacyOutput("Состояние оборудования", multilineBody),
            capturedOutput);
    }

    private static void ExportAllowsEmptyBody()
    {
        ReportDocument document = new ReportDocument("Title without body", string.Empty);

        string capturedOutput = ExportAndCaptureConsoleOutput(document);

        Assertion.AreEqual(
            CreateExpectedLegacyOutput("Title without body", string.Empty),
            capturedOutput);
    }

    private static void ExportRejectsEmptyTitleBeforePrinting()
    {
        ReportDocument document = new ReportDocument(string.Empty, "Test body");
        LegacyReportAdapter reportAdapter = new LegacyReportAdapter(new LegacyReportPrinter());

        string capturedOutput = ConsoleOutputCapture.Capture(
            () => Assertion.Throws<ArgumentException>(() => reportAdapter.Export(document)));

        Assertion.AreEqual(string.Empty, capturedOutput);
    }

    private static void ExportRejectsWhitespaceTitleBeforePrinting()
    {
        ReportDocument document = new ReportDocument("   ", "Test body");
        LegacyReportAdapter reportAdapter = new LegacyReportAdapter(new LegacyReportPrinter());

        string capturedOutput = ConsoleOutputCapture.Capture(
            () => Assertion.Throws<ArgumentException>(() => reportAdapter.Export(document)));

        Assertion.AreEqual(string.Empty, capturedOutput);
    }

    private static void ExportRejectsNullBodyBeforePrinting()
    {
        ReportDocument document = new ReportDocument("Test title", null!);
        LegacyReportAdapter reportAdapter = new LegacyReportAdapter(new LegacyReportPrinter());

        string capturedOutput = ConsoleOutputCapture.Capture(
            () => Assertion.Throws<ArgumentException>(() => reportAdapter.Export(document)));

        Assertion.AreEqual(string.Empty, capturedOutput);
    }

    private static void ExportRejectsNullDocument()
    {
        LegacyReportAdapter reportAdapter = new LegacyReportAdapter(new LegacyReportPrinter());

        ArgumentNullException thrownException = Assertion.Throws<ArgumentNullException>(
            () => reportAdapter.Export(null!));

        Assertion.AreEqual("document", thrownException.ParamName);
    }

    private static void ConstructorRejectsNullLegacyPrinter()
    {
        ArgumentNullException thrownException = Assertion.Throws<ArgumentNullException>(
            () => new LegacyReportAdapter(null!));

        Assertion.AreEqual("legacyReportPrinter", thrownException.ParamName);
    }
}
