using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

public class TaxonomyPage
{
    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;
    
    // Updated locators to match view's HTML structure
    private readonly By _ordersListLocator = By.CssSelector("#OrderCard .list-group-item");
    private readonly By _familyListLocator = By.CssSelector("#FamilyCard .list-group-item");
    private readonly By _dropdownLocator = By.CssSelector("button.dropdown-toggle[data-bs-toggle='dropdown']");
    private readonly By _dropdownMenuLocator = By.CssSelector(".dropdown-menu");
    private readonly By _scientificSortLocator = By.CssSelector("a.dropdown-item[href*='sortOrder=scientific']:not([href*='_desc'])");
    private readonly By _scientificNameLocator = By.CssSelector(".scientific-name");
    private readonly By _birdOrderLocator = By.CssSelector(".bird-order");
    private readonly By _birdFamilyLocator = By.CssSelector(".bird-family");

    public TaxonomyPage(IWebDriver driver)
    {
        _driver = driver;
        _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
    }

    private IReadOnlyCollection<IWebElement> WaitAndFindElements(By locator)
    {
        try
        {
            return _wait.Until(d => {
                var elements = d.FindElements(locator);
                return elements.Any() ? elements : null;
            });
        }
        catch (WebDriverTimeoutException ex)
        {
            Console.WriteLine($"Timeout waiting for elements: {locator}, {ex.Message}");
            return new List<IWebElement>();
        }
    }

    private IWebElement WaitAndFindElement(By locator)
    {
        try
        {
            return _wait.Until(d => {
                var element = d.FindElement(locator);
                return element.Displayed ? element : null;
            });
        }
        catch (WebDriverTimeoutException ex)
        {
            Console.WriteLine($"Timeout waiting for element: {locator}, {ex.Message}");
            throw new NoSuchElementException($"Element not found: {locator}", ex);
        }
    }

    public IList<string> GetOrders()
    {
        var elements = WaitAndFindElements(_ordersListLocator);
        return elements.Select(e => e.Text.Trim()).ToList();
    }

    public IList<string> GetFamilies()
    {
        var elements = WaitAndFindElements(_familyListLocator);
        return elements.Select(e => e.Text.Trim()).ToList();
    }

    public void ClickOrder(string order)
    {
        var elements = WaitAndFindElements(_ordersListLocator);
        var element = elements.FirstOrDefault(e => e.Text.Contains(order));
        
        if (element == null)
            throw new NoSuchElementException($"Order '{order}' not found");

        ScrollAndClick(element);
    }

    public void ClickFamily(string family)
    {
        var elements = WaitAndFindElements(_familyListLocator);
        var element = elements.FirstOrDefault(e => e.Text.Contains(family));
        
        if (element == null)
            throw new NoSuchElementException($"Family '{family}' not found");

        ScrollAndClick(element);
    }

    private void ScrollAndClick(IWebElement element)
    {
        ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", element);
        Thread.Sleep(750); // Allow time for scrolling animation

        _wait.Until(d => {
            try {
                return element.Enabled && element.Displayed;
            }
            catch (StaleElementReferenceException) {
                return false;
            }
        });
        
        element.Click();
    }

    public IList<string> GetBirdOrders()
    {
        return WaitAndFindElements(_birdOrderLocator)
            .Select(e => e.Text.Trim()).ToList();
    }

    public IList<string> GetBirdFamilies()
    {
        return WaitAndFindElements(_birdFamilyLocator)
            .Select(e => e.Text.Trim()).ToList();
    }

    public IList<string> GetScientificNames()
    {
        return WaitAndFindElements(_scientificNameLocator)
            .Select(e => e.Text.Trim()).ToList();
    }

public void SortByScientificName()
    {
        try
        {
            // Wait for dropdown to be ready
            var dropdown = _wait.Until(ExpectedConditions.ElementToBeClickable(_dropdownLocator));
            
            // Ensure page is stable
            Thread.Sleep(500);
            
            // Click with retry
            try 
            {
                dropdown.Click();
            }
            catch (ElementClickInterceptedException)
            {
                ((IJavaScriptExecutor)_driver).ExecuteScript(
                    "arguments[0].dispatchEvent(new Event('click', { 'bubbles': true }));", 
                    dropdown);
            }

            // Wait for Bootstrap to finish showing menu
            var menuShown = _wait.Until(d => 
            {
                var menu = d.FindElement(_dropdownMenuLocator);
                return menu.GetAttribute("class").Contains("show") && menu.Displayed;
            });

            if (!menuShown)
            {
                Console.WriteLine("Menu failed to show after click");
                throw new ElementNotInteractableException("Dropdown menu not visible");
            }

            // Find and click sort option
            var sortOption = _wait.Until(d => d.FindElements(_scientificSortLocator)
                .FirstOrDefault(e => e.Displayed && e.Enabled));

            if (sortOption == null)
            {
                throw new NoSuchElementException("Scientific sort option not found");
            }

            // Click sort option
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", sortOption);

            // Wait for sort to complete
            _wait.Until(ExpectedConditions.StalenessOf(sortOption));
        }
        catch (WebDriverTimeoutException ex)
        {
            Console.WriteLine($"Failed to sort: {ex.Message}");
            Console.WriteLine($"Page title: {_driver.Title}");
            Console.WriteLine($"Current URL: {_driver.Url}");
            throw;
        }
    }
}