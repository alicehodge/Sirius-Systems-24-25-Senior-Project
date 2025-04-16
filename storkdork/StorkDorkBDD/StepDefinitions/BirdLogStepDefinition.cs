using System;
using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Reqnroll;
using System.Linq;
using OpenQA.Selenium.Interactions;



namespace StorkDorkTests.Steps
{
    [Binding]
    public class BirdLogStepDefinition : IDisposable
    {
        private IWebDriver _driver;
        private const string BaseUrl = "http://localhost:5208";
        private readonly WebDriverWait _wait;

        public BirdLogStepDefinition()
        {
            // Start with VISIBLE browser
            _driver = new ChromeDriver();
            _driver.Manage().Window.Maximize();
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));
        }

        /// -------------------------------------------------------------------
        // First Test: Going to Bird Log Sighting Page
        // -------------------------------------------------------------------
        [Given("I am logged in")]
        public void GivenIAmLoggedIn()
        {
            _driver.Navigate().GoToUrl($"{BaseUrl}/Identity/Account/Login");
            
            // Wait for login form
            _wait.Until(d => d.FindElement(By.Id("Input_Email")));
            
            _driver.FindElement(By.Id("Input_Email")).SendKeys("mcaldwell@a.com");
            _driver.FindElement(By.Id("Input_Password")).SendKeys("Mcaldwell_01");
            _driver.FindElement(By.CssSelector("button[type='submit']")).Click();
            
            // Wait for login completion
            _wait.Until(d => d.FindElement(By.LinkText("Home")));
        }

        [When("I navigate to the Bird Log page")]
        public void WhenNavigateToBirdLog()
        {
            // 1. Find the dropdown toggle using the exact ID from HTML
            var dropdownToggle = _wait.Until(d => 
                d.FindElement(By.Id("birdLogDropdown")));
            
            // 2. Scroll into view and click using JavaScript (more reliable)
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", dropdownToggle);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", dropdownToggle);
            
            // 3. Wait for dropdown menu visibility with explicit check
            _wait.Until(d => 
                d.FindElement(By.CssSelector("#birdLogDropdown + .dropdown-menu"))
                .Displayed.Should().BeTrue());
            
            // 4. Find "View Logs" using exact link text and parent relationship
            var viewLogsLink = _wait.Until(d => 
                d.FindElement(By.XPath("//ul[@aria-labelledby='birdLogDropdown']//a[text()='View Logs']")));
            
            // 5. Double-check visibility before clicking
            _wait.Until(d => viewLogsLink.Displayed);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", viewLogsLink);
            
            // 6. Verify page load using URL pattern instead of title
            _wait.Until(d => d.Url.Contains("/BirdLog"));
        }

        [Then("I should see either {string} or existing bird data")]
        public void ThenIShouldSeeEitherOr(string expectedMessage)
        {
            // Check if table exists first
            var hasData = _driver.FindElements(By.CssSelector("table.table-striped")).Count > 0;

            if (hasData)
            {
                // Verify at least one row exists
                var firstRow = _wait.Until(d => 
                    d.FindElement(By.CssSelector("tbody tr")));
                firstRow.Text.Should().NotBeNullOrEmpty();
            }
            else
            {
                // Verify empty message with exact CSS selector
                var message = _wait.Until(d => 
                    d.FindElement(By.CssSelector("p.text-muted.mt-4"))); // Match your HTML
                message.Text.Should().Be(expectedMessage);
            }
        }
        // -------------------------------------------------------------------
        // END OF FIRST TEST
        // -------------------------------------------------------------------


        [When("I navigate to the Bird Log create page")]
        public void WhenNavigateToBirdLogCreate()
        {
           // 1. Find the dropdown toggle using the exact ID from HTML
            var dropdownToggle = _wait.Until(d => d.FindElement(By.Id("birdLogDropdown")));

            // 2. Scroll into view and click using JavaScript
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", dropdownToggle);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", dropdownToggle);

            // 3. Wait for the dropdown menu to be visible; ensure the menu has the "show" class
            _wait.Until(d => 
                d.FindElement(By.XPath("//ul[@aria-labelledby='birdLogDropdown']")).GetAttribute("class").Contains("show"));

            // 4. Find "Create New Log" using a flexible XPath
            var createLogLink = _wait.Until(d => 
                d.FindElement(By.XPath("//ul[@aria-labelledby='birdLogDropdown']//a[contains(normalize-space(.), 'Create New Log')]")));

            // 5. Optionally wait a brief moment if needed (uncomment if necessary)
            // System.Threading.Thread.Sleep(500);

            // 6. Click the "Create New Log" using Actions
            Actions actions = new Actions(_driver);
            actions.MoveToElement(createLogLink).Click().Perform();

            // 7. Verify that the URL indicates we are on the create page
            _wait.Until(d => d.Url.Contains("/Create"));
        }

        [When("I fill in the sighting form with:")]
        public void WhenIFillInTheSightingFormWith(Table table)
        {
            var data = table.Rows.ToDictionary(r => r[0], r => r[1]);
            
            // Handle Bird Species selection (your existing working code)
            var birdSearch = _wait.Until(d => d.FindElement(By.Id("birdSearch")));
            birdSearch.Clear();
            birdSearch.SendKeys(data["Bird Species"]);
            System.Threading.Thread.Sleep(500);
            
            var birdResult = _wait.Until(d =>
            {
                try
                {
                    var container = d.FindElement(By.Id("birdResults"));
                    if (container.Displayed)
                    {
                        var results = container.FindElements(By.CssSelector(".bird-result"));
                        return results.FirstOrDefault();
                    }
                    return null;
                }
                catch (NoSuchElementException)
                {
                    return null;
                }
            });
            
            if (birdResult != null)
            {
                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", birdResult);
            }
            else
            {
                throw new Exception("No bird search results were found for the provided bird species.");
            }
            
            // Improved Location selection
            SelectLocation(data["Location"]);
            
            // Handle Date if present
            if (data.ContainsKey("Date"))
            {
                var dateInput = _wait.Until(d => d.FindElement(By.CssSelector("input[type='date']")));
                ((IJavaScriptExecutor)_driver).ExecuteScript($"arguments[0].value = '{data["Date"]}';", dateInput);
            }
        }

        private void SelectLocation(string location)
        {
            try
            {
                // 1. Find and click the dropdown to open it
                var dropdown = _wait.Until(d => {
                    var element = d.FindElement(By.Id("PnwLocation"));
                    return element.Displayed && element.Enabled ? element : null;
                });
                
                // Click using both normal click and JS click for maximum compatibility
                try { dropdown.Click(); } 
                catch { ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", dropdown); }
                
                // 2. Wait for options to be visible (adjust timeout as needed)
                _wait.Until(d => {
                    var options = d.FindElements(By.CssSelector("#PnwLocation option"));
                    return options.Any(o => o.Displayed);
                });

                // 3. Find and select the specific option
                var option = _wait.Until(d => {
                    // Try different matching strategies
                    var exactMatch = d.FindElements(By.XPath($"//select[@id='PnwLocation']/option[normalize-space()='{location}']"));
                    if (exactMatch.Any()) return exactMatch.First();
                    
                    var containsMatch = d.FindElements(By.XPath($"//select[@id='PnwLocation']/option[contains(normalize-space(), '{location}')]"));
                    return containsMatch.FirstOrDefault();
                });

                // 4. Click the option using multiple methods
                try 
                {
                    option.Click();
                }
                catch 
                {
                    ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true); arguments[0].click();", option);
                }

                // 5. Verify selection
                _wait.Until(d => {
                    var selectedOption = dropdown.FindElement(By.CssSelector("option:checked"));
                    return selectedOption.Text.Contains(location);
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to select location '{location}': {ex.Message}");
            }
        }


        [When("I submit the form")]
        public void WhenISubmitTheForm()
        {
            // 1. Get current session state
            var initialUrl = _driver.Url;
            
            // 2. Find and click submit button
            var submitButton = _wait.Until(d => d.FindElement(By.CssSelector("button[type='submit']")));
            submitButton.Click();
            
            // 3. Wait for navigation with session validation
            _wait.Until(d => {
                if (d.Url.Contains("/Login")) 
                {
                    throw new Exception("Unexpected logout during form submission");
                }
                
                // Check for either URL change OR confirmation elements
                return d.Url != initialUrl || 
                    d.FindElements(By.XPath("//h1[contains(., 'Log Created Successfully')]")).Any();
            });
        }

        [Then("I should see \"Log Created Successfully\" on the confirmation page")]
        public void ThenIShouldSeeConfirmationMessage()
        {
            // 1. Verify we're not on login page
            if (_driver.Url.Contains("/Login")) 
            {
                throw new Exception("User was logged out before confirmation");
            }

            // 2. Verify exact confirmation page elements
            var confirmationTitle = _wait.Until(d => 
                d.FindElement(By.XPath("//h1[contains(., 'Log Created Successfully')]")));
            
            var successLinks = _wait.Until(d => 
                d.FindElements(By.XPath("//a[contains(@class, 'btn-primary') and contains(., 'View Logs')]")));

            confirmationTitle.Text.Should().Contain("Log Created Successfully");
            successLinks.Should().NotBeEmpty();
            
            // 3. Close browser
            _driver.Quit();
        }

    



        public void Dispose()
        {
            _driver?.Quit();
            _driver?.Dispose();
        }
    }
}