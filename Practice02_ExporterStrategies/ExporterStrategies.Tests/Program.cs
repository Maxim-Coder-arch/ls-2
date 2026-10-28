using ExporterStrategies.Tests;

TestRunner testRunner = new TestRunner();

new ReportServiceTests().RegisterTo(testRunner);
new LegacyReportAdapterTests().RegisterTo(testRunner);
new TextFileReportExporterTests().RegisterTo(testRunner);
new ReportExporterContractTests().RegisterTo(testRunner);

return testRunner.RunAll();
