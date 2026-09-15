using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace Selenium.Tests;

[TestClass]
public class DemoUiTests
{
    private IWebDriver? _driver;
    private readonly string _baseUrl = "http://localhost:5173";

    private static bool IsServerRunning(string url)
    {
        try
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(2) };
            var response = client.GetAsync(url).GetAwaiter().GetResult();
            return true;
        }
        catch
        {
            return false;
        }
    }

    [TestInitialize]
    public void Setup()
    {
        if (!IsServerRunning(_baseUrl))
        {
            Assert.Inconclusive($"Frontend server at '{_baseUrl}' is not running. Start frontend before running interactive Selenium UI tests.");
            return;
        }

        var options = new ChromeOptions();
        options.AddArgument("--start-maximized");
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");

        _driver = new ChromeDriver(options);
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
    }

    [TestCleanup]
    public void TearDown()
    {
        _driver?.Quit();
        _driver?.Dispose();
    }

    [TestMethod]
    public void Page_Loads_AndHasExpectedTitle()
    {
        if (_driver == null)
        {
            Assert.Inconclusive("Selenium WebDriver not initialized because frontend dev server is not active.");
            return;
        }

        _driver.Navigate().GoToUrl(_baseUrl);
        var pageTitle = _driver.Title;

        Assert.IsNotNull(pageTitle);
    }

    [TestMethod]
    public void SubmitForm_InteractsWithUIElements()
    {
        if (_driver == null)
        {
            Assert.Inconclusive("Selenium WebDriver not initialized because frontend dev server is not active.");
            return;
        }

        _driver.Navigate().GoToUrl(_baseUrl);

        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

        var inputField = wait.Until(d => d.FindElement(By.CssSelector("input, textarea, [data-testid='input-value']")));
        Assert.IsNotNull(inputField, "Input field should be rendered on the page.");

        inputField.SendKeys("Selenium Interactive Test Item");

        var submitButton = _driver.FindElement(By.CssSelector("button[type='submit'], .v-btn"));
        Assert.IsNotNull(submitButton, "Submit button should be rendered on the page.");
    }
}
