using NUnit.Framework;
using OpenQA.Selenium;
using Reqnroll;
using System;

namespace StorkDorkBDD.StepDefinitions;

[Binding]
public class CommonSteps
{
    private readonly IWebDriver _driver;

    public CommonSteps()
    {
        _driver = GlobalDriverSetup.Driver;
    }

    [Then(@"I should see text ""(.*)""")]
    public void ThenIShouldSeeText(string expectedText)
    {
        Assert.That(_driver.PageSource.Contains(expectedText), Is.True,
            $"Expected to find text '{expectedText}' on the page, but it was not found.");
    }

    [When(@"I navigate to ""(.*)""")]
    public void GivenINavigateTo(string relativeUrl)
    {
        string baseUrl = "http://localhost:5208"; // You can also pull this from a config if needed
        _driver.Navigate().GoToUrl($"{baseUrl}/{relativeUrl.TrimStart('/')}");
    }

    [Given(@"I navigate to Home")]
    public void GivenINavigateToHome()
    {
        _driver.Navigate().GoToUrl("http://localhost:5208");
    }

    [Given(@"I log in as ""(.*)"" with password ""(.*)""")]
    public void GivenILogInAsWithPassword(string email, string password)
    {
        // Navigate to the login page
        _driver.Navigate().GoToUrl("http://localhost:5208/Identity/Account/Login");

        // Fill in the login form
        _driver.FindElement(By.Id("Input_Email")).SendKeys(email);
        _driver.FindElement(By.Id("Input_Password")).SendKeys(password);
        _driver.FindElement(By.Id("login-submit")).Click();
    }
}
