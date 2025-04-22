using OpenQA.Selenium;

namespace StorkDorkBDD.StepDefinitions;

public static class GlobalDriverSetup
{
    public static IWebDriver? Driver { get; set; }
}
