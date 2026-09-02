using Microsoft.Playwright;
using OpenQA.Selenium;

namespace RateDepartment.PageObject;

public abstract class BasePage(IPage page)
{
    protected readonly IPage Page = page;
}