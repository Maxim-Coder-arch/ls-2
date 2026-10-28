# Практика № 2 — решение

## Почему `ReportService` зависит от `IReportExporter`

Сервис отвечает за бизнес-смысл: он знает, что в сводку входят число задач и число ошибок, и формирует `ReportDocument`. Куда документ попадёт, сервису знать не нужно. Зависимость от интерфейса даёт три вещи:

- новый способ экспорта добавляется новым классом, а `ReportService` не открывается для правок;
- сервис проверяется без консоли и файловой системы через `RecordingReportExporter`;
- выбор реализации принимает точка входа, а не бизнес-логика.

## Где выбирается реализация

Выбор находится в `Program.cs` (функция `CreateReportExporter`). В `ReportService` нет ветвления по типу экспортёра и нет ссылок на `LegacyReportPrinter`. Неизвестный режим приводит к `ArgumentException`, которую `Program` превращает в сообщение в `stderr` и код выхода `1`. Ошибки ввода-вывода дают код выхода `2`.

## Контракт экспортёров

Оба экспортёра используют один `ReportDocumentValidator`, поэтому правила совпадают.

| Вход | Поведение |
| --- | --- |
| `null` вместо документа | `ArgumentNullException` |
| Пустой заголовок или заголовок из пробелов | `ArgumentException` |
| `null` вместо тела | `ArgumentException` |
| Пустое тело | Допустимо |
| `null`, пустой путь или путь из пробелов в `TextFileReportExporter` | `ArgumentNullException` для `null`, `ArgumentException` для остальных, при создании объекта |
| Ошибка записи (например, нет папки) | Исключение `IOException` и его наследники передаётся вызывающему коду |

Формат файла: заголовок, перевод строки, тело. Пустого `catch` в решении нет. `Program` перехватывает только `ArgumentException`, `IOException` и `UnauthorizedAccessException`, выводит сообщение и возвращает ненулевой код выхода.

`ReportService` дополнительно проверяет числа: отрицательные значения и случай, когда ошибок больше, чем задач, дают `ArgumentOutOfRangeException` до обращения к экспортёру.

## Как проверено, что `ReportService` не меняется

Тест `сервис отчётов работает с любым экспортёром без изменения кода` пропускает один и тот же вызов `PublishDailySummary` через `RecordingReportExporter`, `TextFileReportExporter` и `LegacyReportAdapter`. Тесты контракта прогоняют оба боевых экспортёра через одни и те же проверки.

Запуск: `dotnet run --project ExporterStrategies.Tests`. Код возврата `0` означает, что все тесты прошли. Тесты используют временные папки, которые удаляются после каждого теста.

## Дополнительно: JSON-экспортёр без изменения `ReportService`

1. Добавить класс `JsonReportExporter : IReportExporter` с конструктором, принимающим путь, и таким же вызовом `ReportDocumentValidator.EnsureIsExportable`.
2. В `Export` сериализовать документ через `System.Text.Json.JsonSerializer.Serialize(document)` и записать результат в файл без перехвата исключений.
3. В `CreateReportExporter` в `Program.cs` добавить ветку `json`, которая возвращает `new JsonReportExporter(path)`.
4. Добавить тесты экспортёра в тот же набор.

`ReportService`, `IReportExporter`, `LegacyReportAdapter` и `TextFileReportExporter` при этом не меняются: изменения ограничены новым классом и одной строкой выбора в точке входа.
