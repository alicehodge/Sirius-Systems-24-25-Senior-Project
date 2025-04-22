using System;
using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Reqnroll;
using StorkDorkBDD.StepDefinitions;

namespace StorkDorkTests.Steps;

[Binding]
public class BirdLogStepDefinition : IDisposable
{
    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;

    public BirdLogStepDefinition()
    {
        _driver = GlobalDriverSetup.Driver;
        _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));
    }

    [When("I navigate to the Bird Log page")]
    public void WhenNavigateToBirdLog()
    {
        // 1. Open bird log dropdown
        var dropdownToggle = _wait.Until(d =>
            d.FindElement(By.CssSelector("a[aria-labelledby='birdLogDropdown']")));

        dropdownToggle.Click();

        // 2. Click "View Logs"
        var viewLogsLink = _wait.Until(d =>
            d.FindElement(By.XPath("//a[contains(text(),'View Logs')]")));
        viewLogsLink.Click();

        // 3. Confirm page loaded
        _wait.Until(d => d.Title.Contains("Bird Sightings"));
    }

    [Then("I should see either {string} or existing bird data")]
    public void ThenIShouldSeeMessage(string expectedText)
    {
        var message = _wait.Until(d =>
            d.FindElement(By.CssSelector("p.text-muted")));
        message.Text.Should().Be(expectedText);
    }

    public void Dispose()
    {
        // Optional cleanup if needed later
    }
}
