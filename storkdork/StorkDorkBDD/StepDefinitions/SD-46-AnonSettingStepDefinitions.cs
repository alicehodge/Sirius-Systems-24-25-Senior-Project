using NUnit.Framework;
using OpenQA.Selenium;
using Reqnroll;
using System;
using System.Runtime.CompilerServices;

namespace StorkDorkBDD.StepDefinitions;

[Binding]
public class AnonymousSettingSteps
{
    private IWebDriver _driver;

    [BeforeScenario]
    public void Setup()
    {
        _driver = GlobalDriverSetup.Driver;
    }

    /* ----- Scenario: Settings page keeps previous choice, and birder is properly anonymous ----- */

    [Given(@"I log in as user {string} with password {string}")]
    public void GivenILogIsAsUserWithPassword(string email, string password)
    {
        _driver.Navigate().GoToUrl("http://localhost:5208/Identity/Account/Login");

        _driver.FindElement(By.Id("Input_Email")).SendKeys(email);
        _driver.FindElement(By.Id("Input_Password")).SendKeys(password);
        _driver.FindElement(By.Id("login-submit")).Click();
    }

    [When(@"I navigate to route {string}")]
    public void INavigateToRoute(string route)
    {
        _driver.Navigate().GoToUrl($"http://localhost:5208{route}");
    }

    [Then(@"I should see the text {string}")]
    public void IShouldSeeTheText(string text)
    {
        Assert.That(_driver.PageSource.Contains(text), Is.True,
            $"Expected to find text '{text}' on the page, but it was not found.");
    }

    [When(@"I confirm the checkbox")]
    public void WhenIConfirmTheCheckbox()
    {
        var checkbox = _driver.FindElement(By.Id("anonymous-birder"));
        if (!checkbox.Selected)
            checkbox.Click();
    }

    [Then(@"I click the submit button")]
    public void IClickTheSubmitButton()
    {
        _driver.FindElement(By.Id("settings-submit")).Click();
    }

    [When(@"I refresh the page")]
    public void IRefreshThePage()
    {
        _driver.Navigate().Refresh();
    }

    [Then(@"I should see the checkbox still checked")]
    public void IShouldSeeTheCheckbocStillChecked()
    {
        var checkbox = _driver.FindElement(By.Id("anonymous-birder"));
        Assert.That(checkbox.Selected);
    }

    /* ----- Scenario: Sightings on the map show anonymous instead of Patricia Rivers ----- */

}