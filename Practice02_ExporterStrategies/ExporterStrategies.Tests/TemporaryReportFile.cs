namespace ExporterStrategies.Tests;

internal sealed class TemporaryReportFile : IDisposable
{
    private const string ReportFileName = "report.txt";

    private readonly string _temporaryDirectoryPath;

    private TemporaryReportFile(string temporaryDirectoryPath)
    {
        _temporaryDirectoryPath = temporaryDirectoryPath;
        ReportFilePath = Path.Combine(temporaryDirectoryPath, ReportFileName);
    }

    public string ReportFilePath { get; }

    public static TemporaryReportFile CreateInExistingDirectory()
    {
        string temporaryDirectoryPath = CreateUniqueTemporaryDirectoryPath();

        Directory.CreateDirectory(temporaryDirectoryPath);

        return new TemporaryReportFile(temporaryDirectoryPath);
    }

    public static TemporaryReportFile CreateInMissingDirectory()
    {
        string temporaryDirectoryPath = CreateUniqueTemporaryDirectoryPath();

        return new TemporaryReportFile(temporaryDirectoryPath);
    }

    public void Dispose()
    {
        if (Directory.Exists(_temporaryDirectoryPath))
        {
            Directory.Delete(_temporaryDirectoryPath, recursive: true);
        }
    }

    private static string CreateUniqueTemporaryDirectoryPath()
    {
        string uniqueDirectoryName = $"report-test-{Guid.NewGuid():N}";

        return Path.Combine(Path.GetTempPath(), uniqueDirectoryName);
    }
}
