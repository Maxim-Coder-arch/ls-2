namespace LegacyReportAdapter;
public sealed class LegacyReportAdapter : IReportExporter
{
    private readonly LegacyReportPrinter _printer;
    public LegacyReportAdapter(LegacyReportPrinter printer)
    {
        // TODO 1: Защитите обязательную зависимость от некорректного значения.
        _printer = printer;
    }
    public void Export(ReportDocument document)
    {
        // TODO 2: Проверьте входной документ и определите поведение при null.
        // TODO 3: Преобразуйте единый документ в старый API (заголовок и тело).
        // TODO 4: Не изменяйте LegacyReportPrinter и не дублируйте формат его вывода.
        _printer.PrintReport(document.Title, document.Body);
    }
}
