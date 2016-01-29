using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WeedSeeker.Web.Test.Functional.Properties;

namespace WeedSeeker.Web.Test.Functional
{
    [TestFixture]
    public class BasicHomeTest
    {
        IWebDriver driver;
        Uri rootUrl;

        [SetUp]
        public void TestSetUp()
        {
            driver = new ChromeDriver();
            rootUrl = Settings.Default.RootUrl;

            driver.Navigate().GoToUrl(rootUrl);
        }


        [Test]
        public void OpenHomePage()
        {
            Assert.That(driver.Url, Is.EqualTo(rootUrl));
            
        }

        [TearDown]
        public void CleanUp()
        {
            driver?.Close();
            driver?.Dispose();
        }
    }
}
