using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Reqnroll;
using System;

namespace StorkDorkBDD.StepDefinitions;

[Binding]
public class Hooks
{
    [BeforeScenario]
    public static void SetupDriver()
    {
        var options = new ChromeOptions();
        options.AddArgument("--headless");
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");

        GlobalDriverSetup.Driver = new ChromeDriver(options);
        GlobalDriverSetup.Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
    }

    [AfterScenario]
    public static void TearDownDriver()
    {
        if (GlobalDriverSetup.Driver != null)
        {
            GlobalDriverSetup.Driver.Quit();
            GlobalDriverSetup.Driver.Dispose();
            GlobalDriverSetup.Driver = null;
        }
    }
}
