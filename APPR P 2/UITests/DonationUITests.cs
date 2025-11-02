
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Xunit;

namespace APPR_P_2.UITests
{
    public class DonationUITests : IDisposable
    {
        private readonly IWebDriver _driver;
        private readonly string _baseUrl = "https://yourapp.azurewebsites.net";

        public DonationUITests()
        {
            var options = new ChromeOptions();
            options.AddArgument("--headless");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");

            _driver = new ChromeDriver(options);
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        }

        [Fact]
        public void DonationPage_LoadsSuccessfully()
        {
            // Act
            _driver.Navigate().GoToUrl($"{_baseUrl}/Donation/Donate");

            // Assert
            Assert.Contains("Donation", _driver.Title);
            Assert.NotNull(_driver.FindElement(By.Id("DonorFirstName")));
            Assert.NotNull(_driver.FindElement(By.Id("DonorLastName")));
            Assert.NotNull(_driver.FindElement(By.Id("DonorEmail")));
        }

        [Fact]
        public void SubmitDonation_ValidData_RedirectsToThankYou()
        {
            // Arrange
            _driver.Navigate().GoToUrl($"{_baseUrl}/Donation/Donate");

            // Act
            _driver.FindElement(By.Id("DonorFirstName")).SendKeys("Test");
            _driver.FindElement(By.Id("DonorLastName")).SendKeys("User");
            _driver.FindElement(By.Id("DonorEmail")).SendKeys("test@example.com");

            var donationTypeSelect = new SelectElement(_driver.FindElement(By.Id("DonationType")));
            donationTypeSelect.SelectByValue("financial");

            _driver.FindElement(By.Id("DonationAmount")).SendKeys("100");

            var paymentSelect = new SelectElement(_driver.FindElement(By.Id("PaymentMethod")));
            paymentSelect.SelectByValue("creditcard");

            _driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            // Assert
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until(d => d.Url.Contains("ThankYou"));

            Assert.Contains("Thank You", _driver.PageSource);
        }

        [Fact]
        public void SubmitDonation_InvalidData_ShowsValidationErrors()
        {
            // Arrange
            _driver.Navigate().GoToUrl($"{_baseUrl}/Donation/Donate");

            // Act - Submit without filling required fields
            _driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            // Assert
            var validationElements = _driver.FindElements(By.CssSelector(".field-validation-error"));
            Assert.True(validationElements.Count > 0);
        }

        public void Dispose()
        {
            _driver.Quit();
            _driver.Dispose();
        }
    }
}