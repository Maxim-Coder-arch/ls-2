namespace ExporterStrategies.Tests;

internal sealed class TestRunner
{
    private readonly List<TestCase> _registeredTestCases = new List<TestCase>();

    public void Register(string name, Action body)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(body);

        _registeredTestCases.Add(new TestCase(name, body));
    }

    public int RunAll()
    {
        int failedTestCount = 0;

        foreach (TestCase testCase in _registeredTestCases)
        {
            string? failureDescription = TryExecute(testCase);

            if (failureDescription is null)
            {
                Console.WriteLine($"PASS — {testCase.Name}");
                continue;
            }

            failedTestCount++;
            Console.WriteLine($"FAIL — {testCase.Name}: {failureDescription}");
        }

        Console.WriteLine($"Провалено тестов: {failedTestCount} из {_registeredTestCases.Count}.");

        return failedTestCount == 0 ? 0 : 1;
    }

    private static string? TryExecute(TestCase testCase)
    {
        try
        {
            testCase.Body();
            return null;
        }
        catch (AssertionFailedException assertionFailure)
        {
            return assertionFailure.Message;
        }
        catch (Exception unexpectedException)
        {
            return $"Unexpected {unexpectedException.GetType().Name}: {unexpectedException.Message}";
        }
    }
}
