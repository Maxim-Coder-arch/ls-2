# ls-2

Практические задания по системному программированию (урок 2) на C#.

## Содержимое

| Папка | Тема |
| --- | --- |
| `Practice01_LegacyReportAdapter` | Подключение старого принтера отчётов через адаптер |
| `Practice02_ExporterStrategies` | Несколько экспортёров через общий интерфейс |

В каждой папке лежат описание задания в `README.md` и разбор решения в `SOLUTION.md`.

## Требования

- .NET SDK 8.0 или новее.

## Запуск

Практика № 1:

```
dotnet run --project Practice01_LegacyReportAdapter/LegacyReportAdapter
dotnet run --project Practice01_LegacyReportAdapter/LegacyReportAdapter.Tests
```

Практика № 2:

```
dotnet run --project Practice02_ExporterStrategies/ExporterStrategies -- console
dotnet run --project Practice02_ExporterStrategies/ExporterStrategies -- file
dotnet run --project Practice02_ExporterStrategies/ExporterStrategies.Tests
```

Проекты с тестами возвращают код выхода `0`, если все проверки прошли, и `1`, если хотя бы одна провалена.

## Ветки

| Ветка | Содержимое |
| --- | --- |
| `practice-01/legacy-report-adapter` | Решение практики № 1 |
| `practice-02/exporter-strategies` | Решение практики № 2 |
| `docs/repository-readme` | Описание репозитория |
