using System;
using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Reqnroll;
using StorkDorkBDD.StepDefinitions;

namespace StorkDorkBDD.StepDefinitions
{
    [Binding]
    public class BirdLogStepDefinition
    {
        private readonly WebDriverWait _wait;

        public BirdLogStepDefinition()
        {
            _wait = new WebDriverWait(
                    GlobalDriverSetup.Driver ?? throw new InvalidOperationException("WebDriver is not initialized."), 
                    TimeSpan.FromSeconds(15));
        }

        [When("I navigate to the Bird Log page")]
        public void WhenNavigateToBirdLog()
        {
            // 1. Open bird log dropdown
            var dropdownToggle = _wait.Until(d =>
                d.FindElement(By.Id("birdLogDropdown")));

            dropdownToggle.Click();

            // 2. Click "View Logs"
            var viewLogsLink = _wait.Until(d =>
                d.FindElement(By.XPath("//a[contains(text(),'View Logs')]")));
            viewLogsLink.Click();

            // 3. Confirm page loaded
            //_wait.Until(d => d.Title.Contains("Bird Sightings"));
        }

        [Then("I should see either {string} or existing bird data")]
        public void ThenIShouldSeeMessage(string expectedText)
        {
            try
            {
                // First try to find the "no sightings" message
                var message = _wait.Until(d => 
                    d.FindElement(By.CssSelector("p.text-muted")));
                message.Text.Should().Be(expectedText);
            }
            catch (WebDriverTimeoutException)
            {
                // If message not found, look for the sightings table
                var table = _wait.Until(d => 
                    d.FindElement(By.CssSelector("table.table")));
                table.Should().NotBeNull("Expected either the message or bird data table");
                
                // Verify table has at least one row
                var rows = _wait.Until(d => 
                    table.FindElements(By.CssSelector("tbody tr")));
                rows.Count.Should().BeGreaterThan(0, "Expected at least one bird sighting in the table");
            }
        }


    }
}
