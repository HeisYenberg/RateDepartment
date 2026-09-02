using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;
using OpenQA.Selenium;
using RateDepartment.Configs;
using RateDepartment.Extensions;
using RateDepartment.PageObject;
using Serilog;

var errorsList = new List<string>();

var settings = new AppSettingsConfig();
var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("AppSettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();
config.Bind(settings);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(config)
    .CreateLogger();

var parallelOptions = new ParallelOptions
{
    MaxDegreeOfParallelism = settings.Organisation.DepartmentsList.Count
};
var passedCount = settings.Organisation.DepartmentsList.ToDictionary(d => d, _ => 0);

using var playwright = await Playwright.CreateAsync();
await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
{
    Headless = true,
    Args =
    [
        "--no-sandbox",
        "--disable-dev-shm-usage",
        "--disable-gpu"
    ]
});

await Parallel.ForEachAsync(settings.Organisation.DepartmentsList, parallelOptions, async (department, cancellationToken) =>
{
    var context = await CreateContextAsync(browser);
    var page = await context.NewPageAsync();
    var questionnairePage = Task.FromResult(new QuestionnairePage(page));

    for (var i = 0; i < Random.Shared.Next(settings.Tries.Min, settings.Tries.Max); i++)
    {
        await Task.Delay(TimeSpan.FromMinutes(Random.Shared.Next(settings.Timeouts.Min, settings.Timeouts.Max)), cancellationToken);

        try
        {
            await page.GotoAsync(settings.Site);
            await questionnairePage
                .SelectOrganisation(settings.Organisation.Name)
                .SelectDepartment(department)
                .SelectStarRating(settings.Organisation.Rating)
                .ClickSubmit()
                .ClickOk();
            passedCount[department]++;

            Log.Information("Отделу {Department} оставлен отзыв {Rating} звезда", department, settings.Organisation.Rating);
        }
        catch (Exception ex) when (ex is WebDriverException or ObjectDisposedException)
        {
            await SafeContextCloseAsync(context);
            context = await CreateContextAsync(browser);
            const string error = "Драйвер выкинул ошибку, произведена перезагрузка";
            errorsList.Add(error);
            Log.Warning(error);
        }
        catch (Exception e)
        {
            var error = $"Не удалось оставить отзыва отделению {department}";
            errorsList.Add(error);
            Log.Error(e, error);
        }
    }

    await SafeContextCloseAsync(context);
});

Console.WriteLine($"Скрипт завершился {(errorsList.Count == 0 ? "без ошибок" : $"c {errorsList.Count} ошибками")}");
Console.WriteLine(settings.Organisation.DepartmentsList.Select(d => $"Отделу {d} проставлено {passedCount[d]} отзывов").Join(Environment.NewLine));
return;

static async Task<IBrowserContext> CreateContextAsync(IBrowser browser)
{
    await Task.Delay(Random.Shared.Next(1000, 5000));

    return await browser.NewContextAsync(new BrowserNewContextOptions
    {
        ViewportSize = new ViewportSize
        {
            Width = 1920,
            Height = 1080
        }
    });
}

static async Task SafeContextCloseAsync(IBrowserContext context)
{
    try
    {
        await context.CloseAsync();
    }
    catch (Exception)
    {
        // ignored
    }
}