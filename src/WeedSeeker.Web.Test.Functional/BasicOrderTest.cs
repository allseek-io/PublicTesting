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
    public class BasicOrderTest : TestBase
    {
        [Test]
        public void OpenHomePageAndCheckOrderSubmitSuccess()
        {
            var prodNameElement = GetSearchTerm();
            prodNameElement.SendKeys( "Chemdawg");

            var amountElement = Driver.FindElement( By.Id( "amount" ) );
            amountElement.SendKeys( "0.01" );

            var priceElement = Driver.FindElement( By.Id( "price" ) );
            priceElement.SendKeys( "1" );

            var submitButton = Driver.FindElement( By.ClassName( "btn-success" ) );
            submitButton.Click();

            var loginElement = Driver.FindElement( By.Id( "username" ) );
            var passwordElement = Driver.FindElement( By.Id( "password" ) );
            
            Expect(loginElement, Not.Null);
            Expect( passwordElement, Not.Null );
        }

        private IWebElement GetSearchTerm()
        {
            return Driver.FindElement( By.Id( "search-term" ) );
        }
    }
}
