using LegacyReportAdapter.Tests;

TestRunner testRunner = new TestRunner();

new LegacyReportAdapterTests().RegisterTo(testRunner);

return testRunner.RunAll();
