using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Reqnroll;
using System;
using System.Collections.Generic;

namespace StorkDorkBDD.StepDefinitions;

[Binding]
public class RangeSubmissionSteps
{
    private readonly IWebDriver _driver;
    private const string BaseUrl = "http://localhost:5208";

    private readonly Dictionary<string, int> _birdIds = new()
    {
        { "Great Blue Heron", 1 },
        { "Northern Cardinal", 2 }
        // Add other birds as needed
    };

    public RangeSubmissionSteps(IWebDriver driver)
    {
        _driver = driver;
    }

    [When(@"I navigate to bird details for ""(.*)""")]
    public void WhenINavigateToBirdDetailsFor(string birdName)
    {
        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        
        if (!_birdIds.TryGetValue(birdName, out int birdId))
        {
            throw new ArgumentException($"Bird '{birdName}' not found in known birds dictionary");
        }

        // Navigate directly to bird detail page
        _driver.Navigate().GoToUrl($"{BaseUrl}/Bird/Detail/{birdId}");
        
        // Verify we're on the correct page
        var pageTitle = wait.Until(driver => 
            driver.FindElement(By.TagName("h2")));
        Assert.That(pageTitle.Text, Contains.Substring(birdName), 
            $"Expected to find {birdName} in page title");
    }

    [When(@"I click ""Submit Range Information""")]
    public void WhenIClickSubmitRangeInformation()
    {
        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        var button = wait.Until(driver => 
            driver.FindElement(By.XPath("//button[contains(text(), 'Submit Range Information')]")));
        button.Click();
    }

    [Then(@"I should see validation error ""(.*)""")]
    public void ThenIShouldSeeValidationError(string errorMessage)
    {
        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        var error = wait.Until(driver => 
            driver.FindElement(By.CssSelector(".text-danger")));
        Assert.That(error.Text, Is.EqualTo(errorMessage));
    }

    [Then(@"I should not see ""Submit Range Information"" button")]
    public void ThenIShouldNotSeeSubmitRangeInformationButton()
    {
        Assert.That(_driver.FindElements(
            By.XPath("//button[contains(text(), 'Submit Range Information')]")).Count, 
            Is.EqualTo(0));
    }

    [Given(@"I have submitted range information for ""(.*)""")]
    public void GivenIHaveSubmittedRangeInformationFor(string birdName)
    {
        WhenINavigateToBirdDetailsFor(birdName);
        WhenIClickSubmitRangeInformation();
        _driver.FindElement(By.Id("RangeDescription"))
            .SendKeys("Test range description for submission status check");
        _driver.FindElement(By.CssSelector("button[type='submit']")).Click();
    }

    [Then(@"the submission should appear in the moderation queue")]
    public void ThenTheSubmissionShouldAppearInTheModerationQueue()
    {
        // Login as moderator to check queue
        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        
        _driver.Navigate().GoToUrl($"{BaseUrl}/Identity/Account/Login");
        _driver.FindElement(By.Id("Input_Email")).SendKeys("admin@storkdork.com");
        _driver.FindElement(By.Id("Input_Password")).SendKeys("Test1234!");
        _driver.FindElement(By.Id("login-submit")).Click();
        
        _driver.Navigate().GoToUrl($"{BaseUrl}/Moderation");
        
        var contentList = wait.Until(driver => 
            driver.FindElement(By.Id("pending-content-list")));
        Assert.That(contentList.Text.Contains("BirdRange"), Is.True);
    }
}