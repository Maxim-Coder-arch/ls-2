namespace ExporterStrategies.Tests;

internal sealed class ReportExporterContractTests
{
    public void RegisterTo(TestRunner testRunner)
    {
        ArgumentNullException.ThrowIfNull(testRunner);

        testRunner.Register(
            "оба экспортёра отклоняют null-документ одинаково",
            EveryExporterRejectsNullDocument);

        testRunner.Register(
            "оба экспортёра отклоняют пустой заголовок одинаково",
            EveryExporterRejectsEmptyTitle);

        testRunner.Register(
            "оба экспортёра отклоняют заголовок из пробелов одинаково",
            EveryExporterRejectsWhitespaceTitle);

        testRunner.Register(
            "оба экспортёра отклоняют null-тело одинаково",
            EveryExporterRejectsNullBody);

        testRunner.Register(
            "оба экспортёра принимают пустое тело",
            EveryExporterAcceptsEmptyBody);
    }

    private static void ForEveryExporter(Action<IReportExporter> verification)
    {
        using TemporaryReportFile temporaryReportFile = TemporaryReportFile.CreateInExistingDirectory();

        IReportExporter[] reportExporters =
        {
            new LegacyReportAdapter(new LegacyReportPrinter()),
            new TextFileReportExporter(temporaryReportFile.ReportFilePath)
        };

        foreach (IReportExporter reportExporter in reportExporters)
        {
            verification(reportExporter);
        }
    }

    private static void EveryExporterRejectsNullDocument()
    {
        ForEveryExporter(reportExporter =>
        {
            ArgumentNullException thrownException = Assertion.Throws<ArgumentNullException>(
                () => reportExporter.Export(null!));

            Assertion.AreEqual("document", thrownException.ParamName);
        });
    }

    private static void EveryExporterRejectsEmptyTitle()
    {
        ForEveryExporter(reportExporter =>
        {
            ReportDocument documentWithEmptyTitle = new ReportDocument(string.Empty, "Test body");

            Assertion.Throws<ArgumentException>(() => reportExporter.Export(documentWithEmptyTitle));
        });
    }

    private static void EveryExporterRejectsWhitespaceTitle()
    {
        ForEveryExporter(reportExporter =>
        {
            ReportDocument documentWithWhitespaceTitle = new ReportDocument("   ", "Test body");

            Assertion.Throws<ArgumentException>(() => reportExporter.Export(documentWithWhitespaceTitle));
        });
    }

    private static void EveryExporterRejectsNullBody()
    {
        ForEveryExporter(reportExporter =>
        {
            ReportDocument documentWithNullBody = new ReportDocument("Test title", null!);

            Assertion.Throws<ArgumentException>(() => reportExporter.Export(documentWithNullBody));
        });
    }

    private static void EveryExporterAcceptsEmptyBody()
    {
        ForEveryExporter(reportExporter =>
        {
            ReportDocument documentWithEmptyBody = new ReportDocument("Title without body", string.Empty);

            ConsoleOutputCapture.Capture(() => reportExporter.Export(documentWithEmptyBody));
        });
    }
}
