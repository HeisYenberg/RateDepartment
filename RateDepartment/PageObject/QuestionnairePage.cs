using RateDepartment.Extensions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace RateDepartment.PageObject;

public class QuestionnairePage(WebDriver driver) : BasePage(driver)
{
    private readonly By _organisationOption = By.Id("id_lpu_id");

    private readonly By _departmentOption = By.Id("id_department_id");

    private readonly By _starRatings = By.CssSelector("*[class*='gl-star-rating--stars']");

    private readonly string _lStarRaiting = "span[data-value='{0}']";

    private readonly By _submitButton = By.Id("submit");

    private readonly By _okButton = By.Id("okButton");

    public QuestionnairePage SelectOrganisation(string organisation)
    {
        var selectElement = new SelectElement(Driver.FindElement(_organisationOption));
        selectElement.SelectByText(organisation);

        return this;
    }

    public QuestionnairePage SelectDepartment(string department)
    {
        var selectElement = new SelectElement(Driver.FindElement(_departmentOption));
        selectElement.SelectByText(department);

        return this;
    }

    public QuestionnairePage SelectStarRating(string rating)
    {
        foreach (var element in Driver.FindElements(_starRatings))
        {
            element.FindElement(By.CssSelector(_lStarRaiting.Format(rating))).Click();
        }

        return this;
    }

    public QuestionnairePage ClickSubmit()
    {
        Driver.WaitAndClick(_submitButton);

        return this;
    }

    public QuestionnairePage ClickOk()
    {
        Driver.WaitAndClick(_okButton);

        return this;
    }
}