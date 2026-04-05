using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace RateDepartment.Extensions;

public static class WebDriverExtensions
{
    public static void WaitAndClick(this WebDriver driver, By locator, int timeoutInSeconds = 10)
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));

        wait.Until(ExpectedConditions.ElementToBeClickable(locator))
            .Click();
    }
}