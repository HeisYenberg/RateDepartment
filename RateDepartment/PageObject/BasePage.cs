using OpenQA.Selenium;

namespace RateDepartment.PageObject;

public abstract class BasePage(WebDriver driver)
{
    protected readonly WebDriver Driver = driver;
}