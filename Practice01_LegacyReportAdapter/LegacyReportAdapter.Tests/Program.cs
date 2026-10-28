using LegacyReportAdapter;
var failed = new List<string>();
void Check(string name, bool condition)
{
    Console.WriteLine($"{(condition ? "PASS" : "FAIL")} — {name}");
    if (!condition) failed.Add(name);
}
var output = new StringWriter();
var original = Console.Out;
try
{
    Console.SetOut(output);
    IReportExporter exporter = new LegacyReportAdapter(new LegacyReportPrinter());
    exporter.Export(new ReportDocument("Test title", "Test body"));
}
finally { Console.SetOut(original); }
var text = output.ToString();
Check("заголовок передан legacy-принтеру", text.Contains("=== Test title ==="));
Check("тело передано legacy-принтеру", text.Contains("Test body"));
Check("сохранён завершающий маркер", text.Contains("=== END OF REPORT ==="));
// TODO 5: Добавьте тест пустого заголовка или тела согласно контракту.
// TODO 6: Добавьте тест null-документа (можно использовать null!).
// TODO 7: Добавьте тест, что конструктор не принимает null-зависимость.
Console.WriteLine($"Провалено базовых проверок: {failed.Count} из 3.");
