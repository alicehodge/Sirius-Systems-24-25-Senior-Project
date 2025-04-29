using Microsoft.Identity.Client;
using NUnit.Framework;
using OpenQA.Selenium;
using Reqnroll;
using OpenQA.Selenium.Support.UI;
using System;
using System.Runtime.CompilerServices;
using System.Linq;

/* If the way that markers are displayed is changed, 
   then the steps + feature file will need updating.
   Likely case that the map will show all user's sightings
   so logging in a picking the first marker may not
   guarantee it is from the logged in user, 
   meaning no guarantee of it being anonymous.
   This will probably just be a reoute change now that I think about it... or not */

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

    [When(@"I navigate to settings {string}")]
    public void INavigateToRouteSettings(string route)
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

    [When(@"I navigate to map {string}")]
    public void WhenINavigateToRouteMap(string route)
    {
        _driver.Navigate().GoToUrl($"http://localhost:5208{route}");
    }

    [Then(@"I should see my sightings and click on a sighting marker")]
    public void IShouldSeeMySightingsAndClick()
    {
        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));

        var sightings = wait.Until(d => d.FindElements(By.ClassName("leaflet-marker-icon"))
                                        .Where(e => e.Displayed && e.Enabled)
                                        .ToList());

        if (sightings.Count == 0)
            throw new Exception("No sighting markers found on the map.");

        var marker = sightings[0];
        var js = (IJavaScriptExecutor)_driver;
        js.ExecuteScript("arguments[0].click();", marker);
    }

    [Then(@"I should see on popup {string}")]
    public void IShouldSeeOnPopUp(string spottedBy)
    {
        var popup = _driver.FindElement(By.ClassName("leaflet-popup-content"));
        Assert.That(popup.Text, Does.Contain(spottedBy));
    }

}