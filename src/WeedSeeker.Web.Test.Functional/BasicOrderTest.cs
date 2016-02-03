using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenQA.Selenium;

namespace WeedSeeker.Web.Test.Functional
{
    [TestFixture]
    public class BasicOrderTest : BaseTest
    {
        [Test]
        public void OpenHomePageAndCheckOrderSubmitSuccess()
        {
            var prodNameElement = Driver.FindElement( By.Id( "search-term" ) );
            prodNameElement.SendKeys( "example" );

            var amountElement = Driver.FindElement( By.Id( "amount" ) );
            amountElement.SendKeys( "0,01" );

            var priceElement = Driver.FindElement( By.Id( "price" ) );
            priceElement.SendKeys( "1" );

            var submitButton = Driver.FindElement( By.ClassName( "btn-success" ) );
            submitButton.Click();

            var loginElement = Driver.FindElement( By.Id( "username" ) );
            var passwordElement = Driver.FindElement( By.Id( "password" ) );
            
            Assert.IsNotNull(loginElement);
            Assert.IsNotNull( passwordElement );
        }
    }
}
