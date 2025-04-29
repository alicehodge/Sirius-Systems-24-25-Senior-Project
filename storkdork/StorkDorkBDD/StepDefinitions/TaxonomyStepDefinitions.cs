using System;
using System.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Reqnroll;

namespace StorkDorkBDD.StepDefinitions;

[Binding]
public class TaxonomyStepDefinitions
{
    private readonly IWebDriver _driver;
    private readonly TaxonomyPage _taxonomyPage;
    private const string BaseUrl = "http://localhost:5208";

    public TaxonomyStepDefinitions()
    {
        _driver = GlobalDriverSetup.Driver ?? throw new InvalidOperationException("WebDriver is not initialized.");
        _taxonomyPage = new TaxonomyPage(_driver);
    }

    [When(@"I navigate to the Taxonomy Browser page")]
    public void WhenINavigateToTheTaxonomyBrowserPage()
    {
        _driver.Navigate().GoToUrl($"{BaseUrl}/Taxonomy");
        // Assert.That(_taxonomyPage.IsLoaded(), Is.True, "Taxonomy page did not load properly");
    }

    [Then(@"I should see a list of bird orders")]
    public void ThenIShouldSeeAListOfBirdOrders()
    {
        var orders = _taxonomyPage.GetOrders();
        Assert.That(orders.Count, Is.GreaterThan(0), "No orders displayed");
    }

    [Then(@"I should see a list of bird families")]
    public void ThenIShouldSeeAListOfBirdFamilies()
    {
        var families = _taxonomyPage.GetFamilies();
        Assert.That(families.Count, Is.GreaterThan(0), "No families displayed");
    }

    [When(@"I click on the order ""(.*)""")]
    public void WhenIClickOnTheOrder(string order)
    {
        _taxonomyPage.ClickOrder(order);
        // Wait for page to load with correct title
        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        wait.Until(d => d.FindElement(By.TagName("h1")).Text.Contains(order));
    }

    [When(@"I click on the family ""(.*)""")]
    public void WhenIClickOnTheFamily(string family)
    {
        _taxonomyPage.ClickFamily(family);
        // Wait for page to load with correct title
        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));
        wait.Until(d => d.FindElement(By.TagName("h1")).Text.Contains(family));
    }

    [Then(@"I should see a list of (.*) birds")]
    public void ThenIShouldSeeAListOfBirds(string group)
    {
        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        var birds = wait.Until(d => d.FindElements(By.CssSelector(".list-group-item")));
        Assert.That(birds.Count, Is.GreaterThan(0), $"No {group} birds displayed");
    }

    [Then(@"each bird should belong to the ""(.*)"" order")]
    public void ThenEachBirdShouldBelongToTheOrder(string order)
    {
        var birdOrders = _taxonomyPage.GetBirdOrders();
        Assert.That(birdOrders, Is.All.EqualTo(order), 
            "Not all birds belong to the expected order");
    }

    [Then(@"each bird should belong to the ""(.*)"" family")]
    public void ThenEachBirdShouldBelongToTheFamily(string family)
    {
        var birdFamilies = _taxonomyPage.GetBirdFamilies();
        Assert.That(birdFamilies, Is.All.EqualTo(family), 
            "Not all birds belong to the expected family");
    }

    [Then(@"I should see sorting options")]
    public void ThenIShouldSeeSortingOptions()
    {
        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        var dropdown = wait.Until(d => 
            d.FindElement(By.CssSelector(".dropdown-toggle")));
        dropdown.Click();
        
        var options = wait.Until(d => 
            d.FindElements(By.CssSelector(".dropdown-menu .dropdown-item")));
        Assert.That(options.Count, Is.GreaterThan(0), "No sorting options displayed");
    }

    [When(@"I click the ""Sort by Scientific Name"" option")]
    public void WhenIClickTheSortByScientificNameOption()
    {
        _taxonomyPage.SortByScientificName();
    }

    [Then(@"the birds should be sorted by scientific name")]
    public void ThenTheBirdsShouldBeSortedByScientificName()
    {
        var scientificNames = _taxonomyPage.GetScientificNames();
        var sortedNames = scientificNames.OrderBy(n => n).ToList();
        Assert.That(scientificNames, Is.EqualTo(sortedNames), 
            "Birds are not sorted by scientific name");
    }
}