using ExporterStrategies;

const string ConsoleExporterMode = "console";
const string FileExporterMode = "file";
const int InvalidArgumentsExitCode = 1;
const int ExportFailureExitCode = 2;

string requestedExporterMode = args.Length > 0 ? args[0] : ConsoleExporterMode;

try
{
    IReportExporter reportExporter = CreateReportExporter(requestedExporterMode);
    ReportService reportService = new ReportService(reportExporter);

    reportService.PublishDailySummary(totalJobs: 42, failedJobs: 2);

    Console.WriteLine($"Использован экспортёр: {reportExporter.GetType().Name}");

    return 0;
}
catch (ArgumentException invalidArgumentsException)
{
    Console.Error.WriteLine(invalidArgumentsException.Message);

    return InvalidArgumentsExitCode;
}
catch (Exception exportFailureException) when (exportFailureException is IOException or UnauthorizedAccessException)
{
    Console.Error.WriteLine($"Не удалось экспортировать отчёт: {exportFailureException.Message}");

    return ExportFailureExitCode;
}

static IReportExporter CreateReportExporter(string exporterMode)
{
    switch (exporterMode.ToLowerInvariant())
    {
        case ConsoleExporterMode:
            return new LegacyReportAdapter(new LegacyReportPrinter());

        case FileExporterMode:
            string dailySummaryFilePath = Path.Combine(Path.GetTempPath(), "daily-summary.txt");
            return new TextFileReportExporter(dailySummaryFilePath);

        default:
            throw new ArgumentException(
                $"Unknown exporter mode '{exporterMode}'. Supported modes: {ConsoleExporterMode}, {FileExporterMode}.",
                nameof(exporterMode));
    }
}
