using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using WeedSeeker.Web.Test.Functional.Pages;

namespace WeedSeeker.Web.Test.Functional.Specs
{
    [TestFixture]
    public class RegisterSpecs : TestBase
    {
        [Test]
        public async Task Signup()
        {
            var randomName = DateTime.Now.ToUniversalTime().Ticks.ToString("x");
            var userName = $"devusr_{randomName}";

            var page = new RegisterPage(Driver);
            page.Navigate();

            page.Username = $"devusr_{randomName}";
            page.Email = $"development+{userName}@weedseeker.net";
            page.Password = "123456";
            page.ConfirmPassword = "123456";
            page.Country = "United States";
            page.State = "California";
            page.City = "San Jose";
            //Agree
            page.Agree.Click();

            await page.WaitAngularAsync();
            page.Submit();

            page.ConfirmationLabel.Should()
                .Be("Account created successfully.");
        }
    }
}
