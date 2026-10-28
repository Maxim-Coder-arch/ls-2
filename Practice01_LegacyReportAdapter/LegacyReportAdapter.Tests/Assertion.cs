namespace LegacyReportAdapter.Tests;

internal static class Assertion
{
    public static void IsTrue(bool condition, string failureMessage)
    {
        if (!condition)
        {
            throw new AssertionFailedException(failureMessage);
        }
    }

    public static void AreEqual<TValue>(TValue expectedValue, TValue actualValue)
    {
        if (!EqualityComparer<TValue>.Default.Equals(expectedValue, actualValue))
        {
            throw new AssertionFailedException(
                $"Expected <{expectedValue}> but was <{actualValue}>.");
        }
    }

    public static void Contains(string expectedFragment, string actualText)
    {
        if (!actualText.Contains(expectedFragment, StringComparison.Ordinal))
        {
            throw new AssertionFailedException(
                $"Expected text to contain <{expectedFragment}> but was <{actualText}>.");
        }
    }

    public static TException Throws<TException>(Action action)
        where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(action);

        Exception? thrownException = null;

        try
        {
            action();
        }
        catch (Exception caughtException)
        {
            thrownException = caughtException;
        }

        if (thrownException is null)
        {
            throw new AssertionFailedException(
                $"Expected exception {typeof(TException).Name} but nothing was thrown.");
        }

        if (thrownException.GetType() != typeof(TException))
        {
            throw new AssertionFailedException(
                $"Expected exception {typeof(TException).Name} but {thrownException.GetType().Name} was thrown.");
        }

        return (TException)thrownException;
    }
}
