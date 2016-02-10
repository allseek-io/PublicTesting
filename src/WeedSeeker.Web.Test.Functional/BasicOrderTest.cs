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
            OpenLoginPageAndSignIn( "kllzn", "123456" );
            
            // Check order creation
            var prodName = "Early Girl";
            var prodNameElement = GetSearchTerm();
            prodNameElement.SendKeys( "Chemdawg");

            var amount = "1";
            var amountElement = Driver.FindElement( By.Id( "amount" ) );
            amountElement.SendKeys( amount );

            var price = "1";
            var priceElement = Driver.FindElement( By.Id( "price" ) );
            priceElement.SendKeys( price );

            var submitButton = Driver.FindElement( By.ClassName( "btn-success" ) );
            submitButton.Click();

            var alert = Driver.FindElement( By.Id( "error_message" ) );
            Assert.AreEqual( "The entry has been added successfully.", alert.Text );
        }

        [Test]
        public void OpenMySeeksAndCancelOrder()
        {
            OpenLoginPageAndSignIn( "kllzn", "123456" );

            var seekingDropDownButton = Driver.FindElement( By.PartialLinkText( "Seeking" ) );
            seekingDropDownButton.Click();

            var mySeeksButton = Driver.FindElement( By.PartialLinkText( "My Seeks" ) );
            mySeeksButton.Click();
            
            var seeksList = Driver.FindElement( By.ClassName( "media-list" ) );
            var seeks = seeksList.FindElements( By.CssSelector( "li.media-item" ) );

            if( seeks.Count > 0 )
            {
                seeks.First().FindElement( By.ClassName( "btn-danger" ) ).Click();

                var modal = Driver.FindElement( By.ClassName( "modal-dialog" ) );
                modal.FindElement( By.CssSelector( "button.btn-primary" ) ).Click();

                var alert = Driver.FindElement( By.Id( "error_message" ) );

                Assert.AreEqual( "The entry has been removed successfully.", alert.Text );
            }
        }
    }
}
