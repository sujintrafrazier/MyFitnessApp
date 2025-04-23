using System;
using OpenQA.Selenium;
using System.Threading; 
using OpenQA.Selenium.Chrome;
using Xunit;
using OpenQA.Selenium.Support.UI;

public class LoginTests : IDisposable
{
    private readonly IWebDriver _driver;
    private readonly string _url;

    public LoginTests()
    {
        ChromeOptions options = new ChromeOptions();
        options.AddArgument("--headless");
        this._driver = new ChromeDriver();
        this._url = "https://localhost:5001/";
    }

    [Fact]
    public void TestValidLogin()
    {
        this._driver.Navigate().GoToUrl(this._url + "Identity/Account/Login");

        var usernameField = this._driver.FindElement(By.Id("Input_Username"));
        var passwordField = this._driver.FindElement(By.Id("Input_Password"));

        this.SimulateTyping(usernameField, "testuser");
        this.SimulateTyping(passwordField, "123456");

        var loginButton = this._driver.FindElement(By.CssSelector("button[type='submit']"));
        loginButton.Click();

        this._driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        Assert.Equal("Home Page - MyFitnessApp", this._driver.Title);

        Thread.Sleep(5000);
    }

    [Fact]
    public void TestMissingPasswordField()
    {
        this._driver.Navigate().GoToUrl(this._url + "Identity/Account/Login");

        var usernameField = this._driver.FindElement(By.Id("Input_Username"));

        this.SimulateTyping(usernameField, "testuser");

        var loginButton = this._driver.FindElement(By.CssSelector("button[type='submit']"));
        loginButton.Click();

        var wait = new WebDriverWait(this._driver, TimeSpan.FromSeconds(5));
        var passwordErrorMessage = wait.Until(driver => driver.FindElement(By.CssSelector("span.text-danger[data-valmsg-for='Input.Password']")));

        Assert.Equal("The Password field is required.", passwordErrorMessage.Text);

        Thread.Sleep(5000);
    }

    [Fact]
    public void TestMissingUsernameField()
    {
        this._driver.Navigate().GoToUrl(this._url + "Identity/Account/Login");

        var passwordField = this._driver.FindElement(By.Id("Input_Password"));

        this.SimulateTyping(passwordField, "123456");

        var loginButton = this._driver.FindElement(By.CssSelector("button[type='submit']"));
        loginButton.Click();

        var wait = new WebDriverWait(this._driver, TimeSpan.FromSeconds(5));
        var usernameErrorMessage = wait.Until(driver => driver.FindElement(By.CssSelector("span.text-danger[data-valmsg-for='Input.Username']")));

        Assert.Equal("The Username field is required.", usernameErrorMessage.Text);

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
