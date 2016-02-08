using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Selenium.WebDriver.Extensions.JQuery;
using WeedSeeker.Web.Test.Functional.Properties;

namespace WeedSeeker.Web.Test.Functional
{
    /// <summary>
    /// Simple health check test. It opens the site and checks if the site is up.
    /// </summary>
    [TestFixture]
    public class BasicHealthCheckTest : TestBase
    {
        /// <summary>
        /// This test opens the home page and checks whether the URL is correct
        /// </summary>
        [Test]
        public void OpenHomePageAndCheckUrl()
        {
            
            Driver.Navigate().GoToUrl(RootUrl);
            Assert.That(Driver.Url, Is.EqualTo(RootUrl));

        }

        /// <summary>
        /// Opens the home page and verifies whether that page is actually the home.
        /// </summary>
        [Test]
        public void OpenHomePageAndVerifyWhetherThatPageIsActuallyTheHome()
        {

            Driver.Navigate().GoToUrl(RootUrl);
            Assert.That(Driver.JQuery("body.home").Count(), Is.Positive);

        }

    }
}
