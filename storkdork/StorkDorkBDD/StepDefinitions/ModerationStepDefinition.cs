using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Reqnroll;
using System;
using System.Linq;

namespace StorkDorkBDD.StepDefinitions;

[Binding]
public class ModerationStepDefinition : IDisposable
{
    private IWebDriver _driver;
    private const string BaseUrl = "http://localhost:5208";

    [BeforeScenario]
    public void Setup()
    {
        var options = new ChromeOptions();
        options.AddArgument("--headless");
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");
        options.AddArgument("--disable-gpu");
        options.AddArgument("--window-size=1920,1080");

        _driver = new ChromeDriver(options);
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        _driver.Manage().Window.Size = new System.Drawing.Size(1920, 1080);
        
        // Clear cookies and cache
        _driver.Manage().Cookies.DeleteAllCookies();
    }

    [Given(@"I am logged in as a moderator")]
    public void GivenIAmLoggedInAsModerator()
    {
        _driver.Navigate().GoToUrl($"{BaseUrl}/Identity/Account/Login");
        _driver.FindElement(By.Id("Input_Email")).SendKeys("admin@storkdork.com");
        _driver.FindElement(By.Id("Input_Password")).SendKeys("Test1234!"); // Use actual test password
        _driver.FindElement(By.Id("login-submit")).Click();
    }

    [Given(@"I am logged in as a regular user")]
    public void GivenIAmLoggedInAsRegularUser()
    {
        _driver.Navigate().GoToUrl($"{BaseUrl}/Identity/Account/Login");
        _driver.FindElement(By.Id("Input_Email")).SendKeys("pasta1234@maildrop.cc");
        _driver.FindElement(By.Id("Input_Password")).SendKeys("Test5678!"); // Use actual test password
        _driver.FindElement(By.Id("login-submit")).Click();
    }

    [Given(@"there is pending content in the moderation queue")]
    public void GivenThereIsPendingContentInTheModerationQueue()
    {
        // manually seeding pending content for now
    }

    [When(@"I navigate to the moderation queue")]
    [When(@"I try to access the moderation queue")]
    public void WhenINavigateToTheModerationQueue()
    {
        _driver.Navigate().GoToUrl($"{BaseUrl}/Moderation");
    }

    [Then(@"I should see a list of pending content")]
    public void ThenIShouldSeeAListOfPendingContent()
    {
        // Add explicit wait for the element
        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        try
        {
            var contentList = wait.Until(driver => driver.FindElement(By.Id("pending-content-list")));
            Assert.That(contentList.Displayed, Is.True, "Content list should be visible");
        }
        catch (WebDriverTimeoutException)
        {
            // Log the page source for debugging
            Console.WriteLine("Page source: " + _driver.PageSource);
            Assert.Fail("Could not find pending-content-list element after 10 seconds");
        }
    }

    [Then(@"I should see content details including ""(.*)"" and ""(.*)""")]
    public void ThenIShouldSeeContentDetailsIncluding(string detail1, string detail2)
    {
        Assert.Multiple(() =>
        {
            Assert.That(_driver.PageSource.Contains(detail1), Is.True);
            Assert.That(_driver.PageSource.Contains(detail2), Is.True);
        });
    }

    [Given(@"I am viewing a pending content item")]
    public void GivenIAmViewingAPendingContentItem()
    {
        _driver.Navigate().GoToUrl($"{BaseUrl}/Moderation/Queue");
        // Click the first pending item in the list
        _driver.FindElement(By.CssSelector(".pending-content-item")).Click();
    }

    [When(@"I click the ""(.*)"" button")]
    public void WhenIClickTheButton(string buttonText)
    {
        _driver.FindElement(By.XPath($"//button[contains(text(),'{buttonText}')]")).Click();
    }

    [When(@"I enter moderator notes ""(.*)""")]
    public void WhenIEnterModeratorNotes(string notes)
    {
        _driver.FindElement(By.Id("moderator-notes")).SendKeys(notes);
    }

    [When(@"I enter rejection reason ""(.*)""")]
    public void WhenIEnterRejectionReason(string reason)
    {
        _driver.FindElement(By.Id("rejection-reason")).SendKeys(reason);
    }

    [When(@"I submit the approval")]
    public void WhenISubmitTheApproval()
    {
        _driver.FindElement(By.Id("submit-approval")).Click();
    }

    [When(@"I submit the rejection")]
    public void WhenISubmitTheRejection()
    {
        _driver.FindElement(By.Id("submit-rejection")).Click();
    }

    [Then(@"the content should be marked as ""(.*)""")]
    public void ThenTheContentShouldBeMarkedAs(string status)
    {
        var statusElement = _driver.FindElement(By.CssSelector(".content-status"));
        Assert.That(statusElement.Text, Is.EqualTo(status));
    }

    [Then(@"I should be redirected to the moderation queue")]
    public void ThenIShouldBeRedirectedToTheModerationQueue()
    {
        Assert.That(_driver.Url, Is.EqualTo($"{BaseUrl}/Moderation"));
    }

    [Then(@"I should be denied access")]
    public void ThenIShouldBeDeniedAccess()
    {
        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));
        try 
        {
            // Wait for redirect to access denied page
            wait.Until(driver => driver.Url.Contains("/Identity/Account/AccessDenied"));
            
            // Verify we're on the access denied page
            Assert.That(_driver.Url.Contains("/Identity/Account/AccessDenied"), Is.True, "Not redirected to access denied page");
            
            // Check for the access denied message
            var headerElement = _driver.FindElement(By.CssSelector("h1.text-danger"));
            Assert.That(headerElement.Text, Is.EqualTo("Access denied"), "Access denied message not found");
            
            // Log current state for debugging
            Console.WriteLine($"Current URL: {_driver.Url}");
            Console.WriteLine($"Page title: {_driver.Title}");
        }
        catch (WebDriverTimeoutException)
        {
            Console.WriteLine($"Current URL: {_driver.Url}");
            Console.WriteLine($"Page source: {_driver.PageSource}");
            Assert.Fail("Timed out waiting for access denied page");
        }
    }

    [When(@"I navigate to the moderation history")]
    public void GivenIAmOnTheModerationHistoryPage()
    {
        _driver.Navigate().GoToUrl($"{BaseUrl}/Moderation/History");
    }

    [When(@"I click the filter button for {string}")]
    public void WhenIClickTheFilterButtonFor(string filterOption)
    {
        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));
        var button = wait.Until(driver => 
            driver.FindElement(By.CssSelector($".filter-buttons button[data-filter='{filterOption}']")));
        button.Click();
    }

    [Then(@"I should only see content with status {string}")]
    public void ThenIShouldOnlySeeContentWithStatus(string status)
    {
        var statuses = _driver.FindElements(By.CssSelector(".content-status"))
            .Select(e => e.Text);
        Assert.That(statuses, Is.All.EqualTo(status));
    }

    [AfterScenario]
    public void TearDown()
    {
        _driver?.Quit();
    }

    public void Dispose()
    {
        _driver?.Quit();
        _driver?.Dispose();
    }
}