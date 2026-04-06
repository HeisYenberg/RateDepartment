using RateDepartment.Configs;
using RateDepartment.Extensions;
using RateDepartment.PageObject;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium.Chrome;
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

var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = settings.Organisation.DepartmentsList.Count };
var random = new Random();
var passedCount = settings.Organisation.DepartmentsList.ToDictionary(d => d, _ => 0);

Parallel.ForEach(settings.Organisation.DepartmentsList, parallelOptions, department =>
{
    var options = new ChromeOptions();
    options.AddArgument("--headless=new");
    options.AddArgument("--no-sandbox");
    options.AddArgument("--disable-dev-shm-usage");
    options.AddArgument("--disable-gpu");
    options.AddArgument("--window-size=1920,1080");

    Thread.Sleep(new Random().Next(1000, 5000));
    using var driver = new ChromeDriver(ChromeDriverService.CreateDefaultService(), options, TimeSpan.FromMinutes(3));
    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(500);
    driver.Manage().Window.Maximize();
    var questionnairePage = new QuestionnairePage(driver);

    for (var i = 0; i < random.Next(settings.Tries.Min, settings.Tries.Max); i++)
    {
        Thread.Sleep(TimeSpan.FromMinutes(random.Next(settings.Timeouts.Min, settings.Timeouts.Max)));

        try
        {
            driver.Navigate().GoToUrl(settings.Site);
            questionnairePage.SelectOrganisation(settings.Organisation.Name)
                .SelectDepartment(department)
                .SelectStarRating(settings.Organisation.Rating)
                .ClickSubmit()
                .ClickOk();
            passedCount[department]++;

            Log.Information("Отделу {Department} оставлен отзыв 1 звезда", department);
        }
        catch (Exception)
        {
            var error = $"Не удалось оставить отзыва отделению {department}";
            errorsList.Add(error);
            Log.Error(error);
        }
    }
});

Console.WriteLine($"Скрипт завершился {(errorsList.Count == 0 ? "без ошибок" : $"c {errorsList.Count} ошибками")}");
Console.WriteLine(settings.Organisation.DepartmentsList.Select(d => $"Отделу {d} проставлено {passedCount[d]} отзывов").Join(Environment.NewLine));