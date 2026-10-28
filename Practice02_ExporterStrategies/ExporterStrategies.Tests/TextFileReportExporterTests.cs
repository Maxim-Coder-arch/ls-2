namespace ExporterStrategies.Tests;

internal sealed class TextFileReportExporterTests
{
    public void RegisterTo(TestRunner testRunner)
    {
        ArgumentNullException.ThrowIfNull(testRunner);

        testRunner.Register(
            "файловый экспорт создаёт файл",
            ExportCreatesReportFile);

        testRunner.Register(
            "файл содержит заголовок",
            ExportedFileContainsTitle);

        testRunner.Register(
            "файл содержит значения отчёта",
            ExportedFileContainsReportValues);

        testRunner.Register(
            "файл записан в согласованном формате заголовок-перевод строки-тело",
            ExportedFileUsesAgreedFormat);

        testRunner.Register(
            "пустое тело записывается как заголовок с переводом строки",
            ExportWritesTitleOnlyForEmptyBody);

        testRunner.Register(
            "повторный экспорт перезаписывает файл",
            ExportOverwritesExistingFile);

        testRunner.Register(
            "конструктор не принимает null-путь",
            ConstructorRejectsNullPath);

        testRunner.Register(
            "конструктор не принимает пустой путь",
            ConstructorRejectsEmptyPath);

        testRunner.Register(
            "конструктор не принимает путь из пробелов",
            ConstructorRejectsWhitespacePath);

        testRunner.Register(
            "ошибка записи в несуществующую папку не скрывается",
            ExportDoesNotSuppressWriteFailure);
    }

    private static void ExportCreatesReportFile()
    {
        using TemporaryReportFile temporaryReportFile = TemporaryReportFile.CreateInExistingDirectory();
        ReportService reportService = new ReportService(new TextFileReportExporter(temporaryReportFile.ReportFilePath));

        reportService.PublishDailySummary(totalJobs: 10, failedJobs: 1);

        Assertion.IsTrue(
            File.Exists(temporaryReportFile.ReportFilePath),
            "Report file was not created.");
    }

    private static void ExportedFileContainsTitle()
    {
        using TemporaryReportFile temporaryReportFile = TemporaryReportFile.CreateInExistingDirectory();
        ReportService reportService = new ReportService(new TextFileReportExporter(temporaryReportFile.ReportFilePath));

        reportService.PublishDailySummary(totalJobs: 10, failedJobs: 1);

        string reportFileContent = File.ReadAllText(temporaryReportFile.ReportFilePath);

        Assertion.Contains("Сводка за день", reportFileContent);
    }

    private static void ExportedFileContainsReportValues()
    {
        using TemporaryReportFile temporaryReportFile = TemporaryReportFile.CreateInExistingDirectory();
        ReportService reportService = new ReportService(new TextFileReportExporter(temporaryReportFile.ReportFilePath));

        reportService.PublishDailySummary(totalJobs: 10, failedJobs: 1);

        string reportFileContent = File.ReadAllText(temporaryReportFile.ReportFilePath);

        Assertion.Contains("Всего задач: 10", reportFileContent);
        Assertion.Contains("Ошибок: 1", reportFileContent);
    }

    private static void ExportedFileUsesAgreedFormat()
    {
        using TemporaryReportFile temporaryReportFile = TemporaryReportFile.CreateInExistingDirectory();
        TextFileReportExporter textFileReportExporter = new TextFileReportExporter(temporaryReportFile.ReportFilePath);

        textFileReportExporter.Export(new ReportDocument("Test title", "Test body"));

        string reportFileContent = File.ReadAllText(temporaryReportFile.ReportFilePath);

        Assertion.AreEqual($"Test title{Environment.NewLine}Test body", reportFileContent);
    }

    private static void ExportWritesTitleOnlyForEmptyBody()
    {
        using TemporaryReportFile temporaryReportFile = TemporaryReportFile.CreateInExistingDirectory();
        TextFileReportExporter textFileReportExporter = new TextFileReportExporter(temporaryReportFile.ReportFilePath);

        textFileReportExporter.Export(new ReportDocument("Title without body", string.Empty));

        string reportFileContent = File.ReadAllText(temporaryReportFile.ReportFilePath);

        Assertion.AreEqual($"Title without body{Environment.NewLine}", reportFileContent);
    }

    private static void ExportOverwritesExistingFile()
    {
        using TemporaryReportFile temporaryReportFile = TemporaryReportFile.CreateInExistingDirectory();
        TextFileReportExporter textFileReportExporter = new TextFileReportExporter(temporaryReportFile.ReportFilePath);

        textFileReportExporter.Export(new ReportDocument("First title", "First body"));
        textFileReportExporter.Export(new ReportDocument("Second title", "Second body"));

        string reportFileContent = File.ReadAllText(temporaryReportFile.ReportFilePath);

        Assertion.AreEqual($"Second title{Environment.NewLine}Second body", reportFileContent);
    }

    private static void ConstructorRejectsNullPath()
    {
        ArgumentNullException thrownException = Assertion.Throws<ArgumentNullException>(
            () => new TextFileReportExporter(null!));

        Assertion.AreEqual("targetFilePath", thrownException.ParamName);
    }

    private static void ConstructorRejectsEmptyPath()
    {
        ArgumentException thrownException = Assertion.Throws<ArgumentException>(
            () => new TextFileReportExporter(string.Empty));

        Assertion.AreEqual("targetFilePath", thrownException.ParamName);
    }

    private static void ConstructorRejectsWhitespacePath()
    {
        ArgumentException thrownException = Assertion.Throws<ArgumentException>(
            () => new TextFileReportExporter("   "));

        Assertion.AreEqual("targetFilePath", thrownException.ParamName);
    }

    private static void ExportDoesNotSuppressWriteFailure()
    {
        using TemporaryReportFile temporaryReportFile = TemporaryReportFile.CreateInMissingDirectory();
        TextFileReportExporter textFileReportExporter = new TextFileReportExporter(temporaryReportFile.ReportFilePath);

        Assertion.Throws<DirectoryNotFoundException>(
            () => textFileReportExporter.Export(new ReportDocument("Test title", "Test body")));

        Assertion.IsTrue(
            !File.Exists(temporaryReportFile.ReportFilePath),
            "Report file must not exist after a failed write.");
    }
}
