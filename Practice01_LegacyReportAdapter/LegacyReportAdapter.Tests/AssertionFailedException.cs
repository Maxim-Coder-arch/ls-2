namespace LegacyReportAdapter.Tests;

internal sealed class AssertionFailedException : Exception
{
    public AssertionFailedException(string failureMessage)
        : base(failureMessage)
    {
    }
}
