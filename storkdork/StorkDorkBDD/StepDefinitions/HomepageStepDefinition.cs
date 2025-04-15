using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Reqnroll;
using System;

namespace StorkDorkBDD.StepDefinitions;

[Binding]
public class HomepageStepDefinition : IDisposable
{
    private IWebDriver _driver;

    [BeforeScenario]
    public void Setup()
    {
        var options = new ChromeOptions();
        options.AddArgument("--headless"); // Run in headless mode (no UI)
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");

        _driver = new ChromeDriver(options);
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
    }

    public void Dispose()
    {
        if (_driver != null)
        {
            _driver.Quit();
            _driver.Dispose();
        }
    }

    [AfterScenario]
    public void Teardown()
    {
        _driver.Quit();
    }

    [Given("I open the home page")]
    public void GivenIOpenTheHomePage()
    {
        _driver.Navigate().GoToUrl("http://localhost:5208");
    }

    [Then(@"I should see homepage text ""(.*)""")]
    public void ThenIShouldSeeHomepageText(string text)
    {
        Assert.That(_driver.PageSource.Contains(text), Is.True);
    }

}