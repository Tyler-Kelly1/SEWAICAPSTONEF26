using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace Selenium.Tests;

[TestClass]
public class WorkoutTemplateAndOverloadTests
{
    private IWebDriver? _driver;
    private readonly string _frontendUrl = "http://localhost:5173";
    private readonly string _backendUrl = "http://localhost:5245/api/WorkoutSession/users";

    private static bool IsServerHealthy(string url)
    {
        try
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(3) };
            var response = client.GetAsync(url).GetAwaiter().GetResult();
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    [TestInitialize]
    public void Setup()
    {
        if (!IsServerHealthy(_frontendUrl))
        {
            Assert.Inconclusive($"Frontend dev server is not running at '{_frontendUrl}'. Please ensure Vite is running.");
            return;
        }

        if (!IsServerHealthy(_backendUrl))
        {
            Assert.Inconclusive($"Backend API is not running at '{_backendUrl}'. Please ensure the .NET backend is running.");
            return;
        }

        var options = new ChromeOptions();
        // Non-headless mode as instructed by project specifications ("I want to see test happen")
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

    private void SetInputValue(IWebElement inputElement, string text)
    {
        if (_driver is IJavaScriptExecutor js)
        {
            js.ExecuteScript("arguments[0].scrollIntoView({ block: 'center' });", inputElement);
        }

        inputElement.Click();
        inputElement.SendKeys(Keys.Control + "a");
        inputElement.SendKeys(Keys.Backspace);
        inputElement.SendKeys(text);

        // Dispatch input and change events to ensure Vue reactive bindings update
        if (_driver is IJavaScriptExecutor jsExec)
        {
            jsExec.ExecuteScript("arguments[0].dispatchEvent(new Event('input', { bubbles: true })); arguments[0].dispatchEvent(new Event('change', { bubbles: true }));", inputElement);
        }
        Thread.Sleep(150);
    }

    private void SelectVuetifyOption(By selectLocator, string optionText, WebDriverWait wait)
    {
        var selectElement = wait.Until(d => d.FindElement(selectLocator));
        if (_driver is IJavaScriptExecutor js)
        {
            js.ExecuteScript("arguments[0].scrollIntoView({ block: 'center' });", selectElement);
        }

        Thread.Sleep(300);

        IWebElement clickTarget;
        try
        {
            clickTarget = selectElement.FindElement(By.CssSelector(".v-field__append-inner, .v-field, .v-field__input"));
        }
        catch
        {
            clickTarget = selectElement;
        }

        new OpenQA.Selenium.Interactions.Actions(_driver).MoveToElement(clickTarget).Click().Perform();
        Thread.Sleep(500);

        // Wait for Vuetify overlay menu to appear
        var option = wait.Until(d =>
        {
            var items = d.FindElements(By.CssSelector(".v-overlay-container .v-list-item, .v-menu .v-list-item, .v-overlay .v-list-item, .v-list-item"));
            var match = items.FirstOrDefault(i => i.Text.Contains(optionText, StringComparison.OrdinalIgnoreCase));
            if (match == null)
            {
                try
                {
                    clickTarget.Click();
                }
                catch { }
            }
            return match;
        });

        Assert.IsNotNull(option, $"Expected option '{optionText}' in Vuetify dropdown menu.");
        new OpenQA.Selenium.Interactions.Actions(_driver).MoveToElement(option).Click().Perform();
        Thread.Sleep(300);

        // Press Escape to dismiss any residual overlay
        try
        {
            _driver?.FindElement(By.TagName("body")).SendKeys(Keys.Escape);
            Thread.Sleep(200);
        }
        catch
        {
            // Ignore if already closed
        }
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
    public void FullFeature_WorkoutTemplate_PerformTwoSessions_AndVerifyOverloadCalculations()
    {
        if (_driver == null)
        {
            Assert.Inconclusive("WebDriver not initialized.");
            return;
        }

        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));
        var templateName = $"Power Overload Test {DateTime.UtcNow.Ticks % 10000}";

        // Navigate to the Overload Application
        _driver.Navigate().GoToUrl(_frontendUrl);
        wait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").ToString() == "complete");
        EnsureLoggedIn("tyler_dev");

        // =========================================================================================
        // STEP 1: Navigate to 'Workout Templates' Tab and Create a New Workout Template
        // =========================================================================================
        var workoutTemplatesTab = wait.Until(d =>
        {
            var tabs = d.FindElements(By.CssSelector("[data-testid='tab-workout-templates'], .v-tab"));
            return tabs.FirstOrDefault(t => t.Text.Contains("Workout Templates") || t.GetAttribute("data-testid") == "tab-workout-templates");
        });
        Assert.IsNotNull(workoutTemplatesTab, "Workout Templates tab should be present.");
        workoutTemplatesTab.Click();
        Thread.Sleep(400);

        // Fill in the Workout Template Name
        var templateNameField = wait.Until(d =>
        {
            var elem = d.FindElement(By.CssSelector("[data-testid='workout-template-name-input']"));
            return elem.TagName.Equals("input", StringComparison.OrdinalIgnoreCase)
                ? elem
                : elem.FindElement(By.CssSelector("input"));
        });
        SetInputValue(templateNameField, templateName);

        // Select the Exercise Template to include (e.g. "Barbell Bench Press")
        SelectVuetifyOption(By.CssSelector("[data-testid='workout-template-exercises-select']"), "Barbell Bench Press", wait);

        // Click 'Create Workout Template'
        var createTemplateBtn = wait.Until(d => d.FindElement(By.CssSelector("[data-testid='create-workout-template-btn'], button[type='submit']")));
        Assert.IsTrue(createTemplateBtn.Enabled, "Create Workout Template button should be enabled after entering name and selecting exercises.");
        createTemplateBtn.Click();

        // Verify template creation via UI notification or list item
        var templateCreatedNotice = wait.Until(d =>
        {
            var text = d.FindElement(By.TagName("body")).Text;
            return text.Contains(templateName, StringComparison.OrdinalIgnoreCase);
        });
        Assert.IsTrue(templateCreatedNotice, $"Newly created template '{templateName}' should be rendered on the page.");

        // =========================================================================================
        // STEP 2: Start Session 1 from the Created Template
        // =========================================================================================
        var startSessionTab = wait.Until(d =>
        {
            var tabs = d.FindElements(By.CssSelector("[data-testid='tab-start-session'], .v-tab"));
            return tabs.FirstOrDefault(t => t.Text.Contains("Start Session") || t.GetAttribute("data-testid") == "tab-start-session");
        });
        startSessionTab.Click();
        Thread.Sleep(400);

        // Select the created workout template from the dropdown
        SelectVuetifyOption(By.CssSelector("[data-testid='select-workout-template']"), templateName, wait);

        // Click 'Begin Workout Session'
        var beginSessionBtn = wait.Until(d => d.FindElement(By.CssSelector("[data-testid='begin-workout-session-btn'], button[type='submit']")));
        beginSessionBtn.Click();

        // Wait for Active Session view to load with set rows
        wait.Until(d => d.FindElements(By.CssSelector("[data-testid='set-row']")).Count > 0);

        // Wait for BE Goal Weights to finish auto-fetching
        wait.Until(d =>
        {
            var goalWeightElements = d.FindElements(By.CssSelector("[data-testid='goal-weight']"));
            return goalWeightElements.Count > 0 && !goalWeightElements[0].Text.Contains("Auto-fetching");
        });

        // In Session 1, perform Set 1 by achieving Overload Ceiling:
        // Barbell Bench Press template has Max_Reps = 10, Weight_Step = 0.05 (5%), Min_Reps = 6.
        // We will execute Set 1 at 200 lbs with 10 reps (hitting ceiling!).
        var set1WeightInput = wait.Until(d =>
        {
            var rows = d.FindElements(By.CssSelector("[data-testid='set-row']"));
            return rows[0].FindElement(By.CssSelector("[data-testid='set-weight-input'] input"));
        });
        SetInputValue(set1WeightInput, "200");

        var set1RepsInput = wait.Until(d =>
        {
            var rows = d.FindElements(By.CssSelector("[data-testid='set-row']"));
            return rows[0].FindElement(By.CssSelector("[data-testid='set-reps-input'] input"));
        });
        SetInputValue(set1RepsInput, "10");

        // Complete Set 1
        var completeSet1Btn = wait.Until(d =>
        {
            var rows = d.FindElements(By.CssSelector("[data-testid='set-row']"));
            return rows[0].FindElement(By.CssSelector("[data-testid='complete-set-btn']"));
        });
        completeSet1Btn.Click();
        Thread.Sleep(300);

        // Complete Set 2 if present
        var setRows = _driver.FindElements(By.CssSelector("[data-testid='set-row']"));
        if (setRows.Count > 1)
        {
            var set2Weight = setRows[1].FindElement(By.CssSelector("[data-testid='set-weight-input'] input"));
            SetInputValue(set2Weight, "200");
            var set2Reps = setRows[1].FindElement(By.CssSelector("[data-testid='set-reps-input'] input"));
            SetInputValue(set2Reps, "8");
            var completeSet2Btn = setRows[1].FindElement(By.CssSelector("[data-testid='complete-set-btn']"));
            completeSet2Btn.Click();
            Thread.Sleep(300);
        }

        // Complete Set 3 if present
        setRows = _driver.FindElements(By.CssSelector("[data-testid='set-row']"));
        if (setRows.Count > 2)
        {
            var set3Weight = setRows[2].FindElement(By.CssSelector("[data-testid='set-weight-input'] input"));
            SetInputValue(set3Weight, "200");
            var set3Reps = setRows[2].FindElement(By.CssSelector("[data-testid='set-reps-input'] input"));
            SetInputValue(set3Reps, "6");
            var completeSet3Btn = setRows[2].FindElement(By.CssSelector("[data-testid='complete-set-btn']"));
            completeSet3Btn.Click();
            Thread.Sleep(300);
        }

        // Finish & Save Session 1
        var finishSession1Btn = wait.Until(d => d.FindElement(By.CssSelector("[data-testid='finish-workout-btn']")));
        finishSession1Btn.Click();

        // Wait for Session 1 to be saved successfully to PostgreSQL and redirect to history
        wait.Until(d =>
        {
            var text = d.FindElement(By.TagName("body")).Text;
            return text.Contains("Successfully recorded Workout Session", StringComparison.OrdinalIgnoreCase);
        });
        Thread.Sleep(1000);

        // =========================================================================================
        // STEP 3: Start Session 2 from the Same Template and Verify Progressive Overload
        // =========================================================================================
        // Switch back to 'Start Session' tab
        startSessionTab = wait.Until(d =>
        {
            var tabs = d.FindElements(By.CssSelector("[data-testid='tab-start-session'], .v-tab"));
            return tabs.FirstOrDefault(t => t.Text.Contains("Start Session") || t.GetAttribute("data-testid") == "tab-start-session");
        });
        startSessionTab.Click();
        Thread.Sleep(600);
        wait.Until(d => d.FindElement(By.CssSelector("[data-testid='select-workout-template']")).Displayed);

        // Select the same template again
        SelectVuetifyOption(By.CssSelector("[data-testid='select-workout-template']"), templateName, wait);

        // Click 'Begin Workout Session' for Session 2
        beginSessionBtn = wait.Until(d => d.FindElement(By.CssSelector("[data-testid='begin-workout-session-btn'], button[type='submit']")));
        beginSessionBtn.Click();

        // Wait for active session view to load
        wait.Until(d => d.FindElements(By.CssSelector("[data-testid='set-row']")).Count > 0);

        // Wait for backend progressive overload calculations to load into the UI
        wait.Until(d =>
        {
            var goalWeights = d.FindElements(By.CssSelector("[data-testid='goal-weight']"));
            return goalWeights.Count > 0 && !goalWeights[0].Text.Contains("Auto-fetching");
        });

        var session2GoalWeightElem = wait.Until(d =>
        {
            var rows = d.FindElements(By.CssSelector("[data-testid='set-row']"));
            return rows[0].FindElement(By.CssSelector("[data-testid='goal-weight']"));
        });

        var session2GoalRepsElem = wait.Until(d =>
        {
            var rows = d.FindElements(By.CssSelector("[data-testid='set-row']"));
            return rows[0].FindElement(By.CssSelector("[data-testid='goal-reps']"));
        });

        var goalWeightText = session2GoalWeightElem.Text;
        var goalRepsText = session2GoalRepsElem.Text;

        // =========================================================================================
        // VERIFY PROGRESSIVE OVERLOAD CALCULATIONS:
        // Session 1 Set 1: Previous Weight = 200, Previous Reps = 10 (hitting Max_Reps = 10).
        // Overload Ceiling Rule:
        // New Goal Weight = Math.Round(200 * (1 + 0.05)) = 210 lbs.
        // New Goal Reps = Min_Reps = 6 reps.
        // =========================================================================================
        Assert.IsTrue(
            goalWeightText.Contains("210"),
            $"Expected Session 2 Set 1 Goal Weight to be 210 lbs (+5% progressive overload), but was '{goalWeightText}'."
        );

        Assert.IsTrue(
            goalRepsText.Contains("6"),
            $"Expected Session 2 Set 1 Goal Reps to reset to Min_Reps (6 reps) upon hitting ceiling, but was '{goalRepsText}'."
        );

        // Complete Session 2 sets with progressive overload values
        var session2Set1WeightInput = wait.Until(d =>
        {
            var rows = d.FindElements(By.CssSelector("[data-testid='set-row']"));
            return rows[0].FindElement(By.CssSelector("[data-testid='set-weight-input'] input"));
        });
        SetInputValue(session2Set1WeightInput, "210");

        var session2Set1RepsInput = wait.Until(d =>
        {
            var rows = d.FindElements(By.CssSelector("[data-testid='set-row']"));
            return rows[0].FindElement(By.CssSelector("[data-testid='set-reps-input'] input"));
        });
        SetInputValue(session2Set1RepsInput, "6");

        var session2CompleteSet1Btn = wait.Until(d =>
        {
            var rows = d.FindElements(By.CssSelector("[data-testid='set-row']"));
            return rows[0].FindElement(By.CssSelector("[data-testid='complete-set-btn']"));
        });
        session2CompleteSet1Btn.Click();
        Thread.Sleep(300);

        // Finish & Save Session 2
        var finishSession2Btn = wait.Until(d => d.FindElement(By.CssSelector("[data-testid='finish-workout-btn']")));
        finishSession2Btn.Click();

        wait.Until(d =>
        {
            var text = d.FindElement(By.TagName("body")).Text;
            return text.Contains("Successfully recorded Workout Session", StringComparison.OrdinalIgnoreCase);
        });
    }
}
