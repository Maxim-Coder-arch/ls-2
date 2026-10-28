namespace ExporterStrategies.Tests;

internal static class ConsoleOutputCapture
{
    public static string Capture(Action action)
    {
        ArgumentNullException.ThrowIfNull(action);

        TextWriter originalConsoleOutput = Console.Out;
        StringWriter capturedConsoleOutput = new StringWriter();

        try
        {
            Console.SetOut(capturedConsoleOutput);
            action();
        }
        finally
        {
            Console.SetOut(originalConsoleOutput);
        }

        return capturedConsoleOutput.ToString();
    }
}
