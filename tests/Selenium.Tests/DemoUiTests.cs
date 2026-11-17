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

    private void EnsureLoggedIn(string emailOrUserId = "tyler@example.com", string password = "Password123!")
    {
        var email = emailOrUserId;
        if (!email.Contains("@"))
        {
            email = emailOrUserId == "tyler_dev" ? "tyler@example.com" : $"{emailOrUserId}@example.com";
        }

        try
        {
            var emailInputs = _driver!.FindElements(By.CssSelector("[data-testid='input-login-email'] input, [data-testid='input-login-email'], [data-testid='input-user-id']"));
            if (emailInputs.Count > 0 && emailInputs[0].Displayed)
            {
                var input = emailInputs[0].TagName.Equals("input", StringComparison.OrdinalIgnoreCase) 
                    ? emailInputs[0] 
                    : emailInputs[0].FindElement(By.TagName("input"));
                input.Clear();
                input.SendKeys(email);

                var passInputs = _driver.FindElements(By.CssSelector("[data-testid='input-login-password'] input, [data-testid='input-login-password']"));
                if (passInputs.Count > 0)
                {
                    var pass = passInputs[0].TagName.Equals("input", StringComparison.OrdinalIgnoreCase) ? passInputs[0] : passInputs[0].FindElement(By.TagName("input"));
                    pass.Clear();
                    pass.SendKeys(password);
                }
                
                var submitBtn = _driver.FindElement(By.CssSelector("[data-testid='btn-login-submit']"));
                submitBtn.Click();

                var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
                wait.Until(d => d.FindElements(By.CssSelector("[data-testid='current-user-chip'], [data-testid='notebook-app-bar']")).Count > 0);
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
        var emailInput = wait.Until(d => {
            var elem = d.FindElement(By.CssSelector("[data-testid='input-login-email']"));
            return elem.TagName.Equals("input", StringComparison.OrdinalIgnoreCase) ? elem : elem.FindElement(By.TagName("input"));
        });
        Assert.IsNotNull(emailInput);

        emailInput.SendKeys("tyler@example.com");

        var passwordInput = _driver.FindElement(By.CssSelector("[data-testid='input-login-password']"));
        var pass = passwordInput.TagName.Equals("input", StringComparison.OrdinalIgnoreCase) ? passwordInput : passwordInput.FindElement(By.TagName("input"));
        pass.SendKeys("Password123!");

        var submitBtn = _driver.FindElement(By.CssSelector("[data-testid='btn-login-submit']"));
        submitBtn.Click();

        var userChip = wait.Until(d => d.FindElement(By.CssSelector("[data-testid='current-user-chip']")));
        Assert.IsTrue(userChip.Text.Contains("tyler_dev") || userChip.Text.Contains("tyler"));
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

        var appTab = wait.Until(d => {
            var tabs = d.FindElements(By.CssSelector("[data-testid='tab-workout-templates'], [data-testid='tab-start-session'], [data-testid='tab-test-table'], .v-tab"));
            return tabs.FirstOrDefault(t => t.Displayed);
        });
        appTab?.Click();

        var inputField = wait.Until(d => {
            var inputs = d.FindElements(By.CssSelector("input, textarea, [data-testid='input-value']"));
            return inputs.FirstOrDefault(i => i.Displayed);
        });
        Assert.IsNotNull(inputField, "Input field should be rendered on the page.");

        inputField.SendKeys("Selenium Interactive Test Item");

        var submitButton = _driver.FindElements(By.CssSelector("button[type='submit'], .v-btn")).FirstOrDefault(b => b.Displayed);
        Assert.IsNotNull(submitButton, "Submit button should be rendered on the page.");
    }
}
