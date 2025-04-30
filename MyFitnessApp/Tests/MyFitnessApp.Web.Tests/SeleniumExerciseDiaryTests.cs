using System;
using OpenQA.Selenium;
using System.Threading; 
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Chrome;
using Xunit;

public class ExerciseDiaryTests : IDisposable
{
    private readonly IWebDriver _driver;
    private readonly string _url;

    public ExerciseDiaryTests()
    {
        ChromeOptions options = new ChromeOptions();
        options.AddArgument("--headless");
        this._driver = new ChromeDriver();
        this._url = "https://localhost:5001/";
    }

    [Fact]
    public void TestValidExerciseDiary()
    {
        this._driver.Navigate().GoToUrl(this._url + "Identity/Account/Login");

        var usernameField = this._driver.FindElement(By.Id("Input_Username"));
        var passwordField = this._driver.FindElement(By.Id("Input_Password"));

        this.SimulateTyping(usernameField, "testuser");
        this.SimulateTyping(passwordField, "123456");

        var loginButton = this._driver.FindElement(By.CssSelector("button[type='submit']"));
        loginButton.Click();

        this._driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        this._driver.Navigate().GoToUrl(this._url + "Exercises/All");
        this._driver.Navigate().GoToUrl(this._url + "Exercises/Add/60");

        var wait = new WebDriverWait(this._driver, TimeSpan.FromSeconds(10));

        var weightField = wait.Until(d => d.FindElement(By.Id("Weight")));
        weightField.Clear();
        weightField.SendKeys("150");

        var repetitionsField = wait.Until(d => d.FindElement(By.Id("Repetitions")));
        repetitionsField.Clear();
        repetitionsField.SendKeys("10");

        var setsField = wait.Until(d => d.FindElement(By.Id("Sets")));
        setsField.Clear();
        setsField.SendKeys("3");

        var weekdaySelect = new SelectElement(this._driver.FindElement(By.Id("WeekDay")));
        weekdaySelect.SelectByValue("2");

        this._driver.FindElement(By.CssSelector("input[type='submit']")).Click();
        Assert.Equal("Tuesday - MyFitnessApp", this._driver.Title);

        Thread.Sleep(5000);
    }

    [Fact]
    public void TestInvalidWeights()
    {
        this._driver.Navigate().GoToUrl(this._url + "Identity/Account/Login");

        var usernameField = this._driver.FindElement(By.Id("Input_Username"));
        var passwordField = this._driver.FindElement(By.Id("Input_Password"));

        this.SimulateTyping(usernameField, "testuser");
        this.SimulateTyping(passwordField, "123456");

        var loginButton = this._driver.FindElement(By.CssSelector("button[type='submit']"));
        loginButton.Click();

        this._driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        this._driver.Navigate().GoToUrl(this._url + "Exercises/All");
        this._driver.Navigate().GoToUrl(this._url + "Exercises/Add/59");

        var wait = new WebDriverWait(this._driver, TimeSpan.FromSeconds(10));

        var weightField = wait.Until(d => d.FindElement(By.Id("Weight")));
        weightField.Clear();
        weightField.SendKeys("-100");

        var repetitionsField = wait.Until(d => d.FindElement(By.Id("Repetitions")));
        repetitionsField.Clear();
        repetitionsField.SendKeys("5");

        var setsField = wait.Until(d => d.FindElement(By.Id("Sets")));
        setsField.Clear();
        setsField.SendKeys("3");

        var weekdaySelect = new SelectElement(this._driver.FindElement(By.Id("WeekDay")));
        weekdaySelect.SelectByValue("1");

        this._driver.FindElement(By.CssSelector("input[type='submit']")).Click();

        var weightErrorMessage = wait.Until(d => d.FindElement(By.CssSelector("span[data-valmsg-for='Weight']"))).Text;

        Assert.Equal("The value must be between 0 and 500", weightErrorMessage);

        Thread.Sleep(5000);
    }
    private void SimulateTyping(IWebElement element, string text)
    {
        foreach (char character in text)
        {
            element.SendKeys(character.ToString());
            Thread.Sleep(150);
        }
    }

    public void Dispose()
    {
        this._driver.Quit();
    }
}
