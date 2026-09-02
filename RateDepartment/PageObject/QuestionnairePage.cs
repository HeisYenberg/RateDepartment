using Microsoft.Playwright;
using RateDepartment.Extensions;

namespace RateDepartment.PageObject;

public class QuestionnairePage(IPage page) : BasePage(page)
{
    public ILocator OrganisationOption => Page.Locator("#id_lpu_id");

    public ILocator DepartmentOption => Page.Locator("#id_department_id");

    public ILocator StarRatings => Page.Locator("*[class*='gl-star-rating--stars']");

    public ILocator SubmitButton => Page.Locator("#submit");

    public ILocator OkButton => Page.Locator("#okButton");
}

public static class QuestionnairePageExtension
{
    private const string StarRatingSelector = "span[data-value='{0}']";

    public static async Task<QuestionnairePage> SelectOrganisation(this Task<QuestionnairePage> pageTask, string organisation)
    {
        var page = await pageTask;
        await page.OrganisationOption.SelectOptionAsync(new SelectOptionValue { Label = organisation });
        return page;
    }

    public static async Task<QuestionnairePage> SelectDepartment(this Task<QuestionnairePage> pageTask, string department)
    {
        var page = await pageTask;
        await page.DepartmentOption
            .SelectOptionAsync(new SelectOptionValue { Label = department });
        return page;
    }

    public static async Task<QuestionnairePage> SelectStarRating(this Task<QuestionnairePage> pageTask, string rating)
    {
        var page = await pageTask;
        foreach (var element in await page.StarRatings.AllAsync())
        {
            await element
                .Locator(StarRatingSelector.Format(rating))
                .ClickAsync();
        }

        return page;
    }

    public static async Task<QuestionnairePage> ClickSubmit(this Task<QuestionnairePage> pageTask)
    {
        var page = await pageTask;
        await page.SubmitButton.ClickAsync();
        return page;
    }

    public static async Task<QuestionnairePage> ClickOk(this Task<QuestionnairePage> pageTask)
    {
        var page = await pageTask;
        await page.OkButton.ClickAsync();
        return page;
    }
}