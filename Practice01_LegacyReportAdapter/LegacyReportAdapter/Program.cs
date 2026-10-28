using LegacyReportAdapter;
using ReportExporterAdapter = LegacyReportAdapter.LegacyReportAdapter;

LegacyReportPrinter legacyReportPrinter = new LegacyReportPrinter();
IReportExporter reportExporter = new ReportExporterAdapter(legacyReportPrinter);

ReportDocument equipmentStateDocument = new ReportDocument(
    "Состояние оборудования",
    "Всего позиций: 18\nВ ремонте: 3\nСписано: 1");

reportExporter.Export(equipmentStateDocument);
