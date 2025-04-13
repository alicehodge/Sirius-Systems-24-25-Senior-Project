using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Reqnroll;
using System;

namespace StorkDorkBDD.StepDefinitions;

[Binding]
public class MilestoneStepDefinition : IDisposable
{
    private IWebDriver _driver;

    [BeforeScenario]
    public void Setup()
    {
        var options = new ChromeOptions();
        options.AddArgument("--headless");
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

    [Given(@"I open the milestone page as (.*)")]
    public void GivenIOpenTheMilestonePageAs(string email)
    {
        // Navigate to the login page
        _driver.Navigate().GoToUrl("http://localhost:5208/Identity/Account/Login");

        // Fill in the login form
        _driver.FindElement(By.Id("Input_Email")).SendKeys(email);
        _driver.FindElement(By.Id("Input_Password")).SendKeys("676770Winger!");
        _driver.FindElement(By.Id("login-submit")).Click();

        // Navigate to the milestone page
        _driver.Navigate().GoToUrl("http://localhost:5208/Milestone");
    }

    [Then(@"I should see milestone text ""(.*)""")]
    public void ThenIShouldSeeMilestoneText(string text)
    {
        Assert.That(_driver.PageSource.Contains(text), Is.True);
    }
}
