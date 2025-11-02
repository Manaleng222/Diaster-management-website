
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;

public class DisasterAppUITests : IClassFixture<WebDriverFixture>
{
    private readonly IWebDriver _driver;
    private readonly string _baseUrl;

    public DisasterAppUITests(WebDriverFixture fixture)
    {
        _driver = fixture.Driver;
        _baseUrl = "https://disaster-relief-app-h4bvgfgzbneshbd5.southafricanorth-01.azurewebsites.net/";
    }

    [Fact]
    public void HomePage_LoadsSuccessfully()
    {
        // Act
        _driver.Navigate().GoToUrl(_baseUrl);

        // Assert
        Assert.Equal("Disaster Relief App", _driver.Title);
        Assert.Contains("Disaster Management", _driver.PageSource);
    }

    [Fact]
    public void CreateDisasterEvent_FormSubmission_WorksCorrectly()
    {
        // Arrange
        _driver.Navigate().GoToUrl(_baseUrl + "/disaster-events/create");

        // Act
        _driver.FindElement(By.Id("Title")).SendKeys("UI Test Disaster");
        _driver.FindElement(By.Id("Location")).SendKeys("Test Location");
        _driver.FindElement(By.Id("Description")).SendKeys("UI Test Description");
        _driver.FindElement(By.Id("severity")).SendKeys("Medium");
        _driver.FindElement(By.CssSelector("button[type='submit']")).Click();

        // Assert
        var successMessage = _driver.FindElement(By.ClassName("alert-success"));
        Assert.True(successMessage.Displayed);
        Assert.Contains("successfully created", successMessage.Text.ToLower());
    }

    [Fact]
    public void Navigation_Menu_WorksCorrectly()
    {
        // Act
        _driver.Navigate().GoToUrl(_baseUrl);
        _driver.FindElement(By.LinkText("Disaster Events")).Click();

        // Assert
        Assert.Contains("disaster-events", _driver.Url.ToLower());
        Assert.True(_driver.FindElement(By.ClassName("events-list")).Displayed);
    }
}

public class WebDriverFixture : IDisposable
{
    public IWebDriver Driver { get; private set; }

    public WebDriverFixture()
    {
        Driver = new ChromeDriver();
        Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
    }

    public void Dispose()
    {
        Driver.Quit();
        Driver.Dispose();
    }
}
