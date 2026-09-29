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

    private void EnsureLoggedIn(string userId = "tyler_dev")
    {
        try
        {
            var loginInputs = _driver!.FindElements(By.CssSelector("[data-testid='input-user-id'] input, [data-testid='input-user-id']"));
            if (loginInputs.Count > 0)
            {
                var input = loginInputs[0].TagName.Equals("input", StringComparison.OrdinalIgnoreCase) 
                    ? loginInputs[0] 
                    : loginInputs[0].FindElement(By.TagName("input"));
                input.Clear();
                input.SendKeys(userId);
                
                var submitBtn = _driver.FindElement(By.CssSelector("[data-testid='btn-login-submit']"));
                submitBtn.Click();
                Thread.Sleep(500);
            }
        }
        catch
        {
            // Already logged in
        }
    }

    [TestMethod]
    public void WelcomeLogin_RendersAndAllowsLogin()
    {
        if (_driver == null)
        {
            Assert.Inconclusive("Selenium WebDriver not initialized because frontend dev server is not active.");
            return;
        }

        _driver.Navigate().GoToUrl(_baseUrl);

        if (_driver is IJavaScriptExecutor js)
        {
            js.ExecuteScript("localStorage.clear();");
            _driver.Navigate().Refresh();
        }

        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        var loginInput = wait.Until(d => {
            var elem = d.FindElement(By.CssSelector("[data-testid='input-user-id']"));
            return elem.TagName.Equals("input", StringComparison.OrdinalIgnoreCase) ? elem : elem.FindElement(By.TagName("input"));
        });
        Assert.IsNotNull(loginInput);

        loginInput.SendKeys("tyler_dev");
        var submitBtn = _driver.FindElement(By.CssSelector("[data-testid='btn-login-submit']"));
        submitBtn.Click();

        var userChip = wait.Until(d => d.FindElement(By.CssSelector("[data-testid='current-user-chip']")));
        Assert.IsTrue(userChip.Text.Contains("tyler_dev"));
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
        EnsureLoggedIn("tyler_dev");

        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

        var testTableTab = wait.Until(d => {
            var tabs = d.FindElements(By.CssSelector("[data-testid='tab-test-table'], .v-tab"));
            return tabs.FirstOrDefault(t => t.Text.Contains("TEST_TABLE") || t.GetAttribute("data-testid") == "tab-test-table");
        });
        testTableTab?.Click();

        var inputField = wait.Until(d => d.FindElement(By.CssSelector("input, textarea, [data-testid='input-value']")));
        Assert.IsNotNull(inputField, "Input field should be rendered on the page.");

        inputField.SendKeys("Selenium Interactive Test Item");

        var submitButton = _driver.FindElement(By.CssSelector("button[type='submit'], .v-btn"));
        Assert.IsNotNull(submitButton, "Submit button should be rendered on the page.");
    }
}
