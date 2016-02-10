using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using WeedSeeker.Web.Test.Functional.Properties;

namespace WeedSeeker.Web.Test.Functional
{
    public class TestBase : AssertionHelper
    {
        /// <summary>
        /// Gets the instance of Selenium <see cref="IWebDriver"/>.
        /// </summary>
        /// <value>The current instance of Selenium <see cref="IWebDriver"/>.</value>
        public IWebDriver Driver { get; private set; }

        /// <summary>
        /// Gets the root URL of the application being tested.
        /// </summary>
        /// <value>The root URL of the application being tested.</value>
        public Uri RootUrl { get; private set; }

        /// <summary>
        /// Sets up before executing all tests in this fixture
        /// </summary>
        [OneTimeSetUp]
        public void SetUpEnvironmentBeforeAllTests()
        {
            Driver = GetDriver();

            RootUrl = Settings.Default.RootUrl;
        }

        /// <summary>
        /// Gets the instance of the currently configured webdriver.
        /// </summary>
        /// <returns>IWebDriver.</returns>
        private IWebDriver GetDriver()
        {
            IWebDriver result;

            switch (Settings.Default.Driver)
            {
                case "IE":
                    result = GetIEDriver();
                    break;
                default:
                    result = GetChromeDriver();
                    break;
            }
            return result;

        }

        /// <summary>
        /// Gets the instance of the Web driver.
        /// </summary>
        private IWebDriver GetChromeDriver()
        {
            //
            // Currently using ChromeDriver directly. 
            // In the future, we should encapsulate it and use Autofac 
            // dependency injection to instantiate any browser driver.
            //

            var options = new ChromeOptions();

            if (Settings.Default.UseFiddler)
            {
                options.Proxy = new Proxy {HttpProxy = "127.0.0.1:8888"};
            }

            return new ChromeDriver(options);
        }

        /// <summary>
        /// Gets the instance of the Web driver.
        /// </summary>
        private IWebDriver GetIEDriver()
        {

            return new InternetExplorerDriver();
        }

        /// <summary>
        /// Setups the environment before each test.
        /// </summary>
        [SetUp]
        public void SetupEnvironmentBeforeEachTest()
        {
            Driver.Navigate().GoToUrl( RootUrl );
        }

        /// <summary>
        /// Cleans up enviroment after each test.
        /// </summary>
        [TearDown]
        public void CleanUpEnviromentAfterEachTest()
        {
            // Right now, the only cleanup action that happens is a simple logout.
            Driver.Navigate().GoToUrl("/wp-login.php?action=logout");
        }



        /// <summary>
        /// Cleans up the environment after executing all tests in this fixture.
        /// </summary>
        [OneTimeTearDown]
        public void CleanUpEnvironmentAfterAllTests()
        {
            Driver.Quit();
        }

        // This method could be marked with SetUp attribute with specified parameters
        public void OpenLoginPageAndSignIn( string username, string password )
        {
            var mainPageLoginButton = Driver.FindElement( By.LinkText( "Login" ) );
            mainPageLoginButton.Click();

            var userNameField = Driver.FindElement( By.Id( "username" ) );
            userNameField.SendKeys( username );

            var passwordField = Driver.FindElement( By.Id( "password" ) );
            passwordField.SendKeys( password );

            var submitButton = Driver.FindElement( By.Name( "submit" ) );
            submitButton.Click();
        }

        /// <summary>
        /// Finds the logout button if available, and clicks it.
        /// </summary>
        public void Logout()
        {
            var logoutLink = Driver.FindElementOrDefault(By.LinkText("Logout"));
            logoutLink?.Click();
        }
    }
}
