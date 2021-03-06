﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace WeedSeeker.Web.Test.Functional
{
    [TestFixture]
    public class BasicRegisterLoginTest : TestBase
    {   
        [Test]
        public void OpenRegisterPageAndSignUp()
        {

            var randomName = CreateRandomName();

            Logout();


            var mainPageRegisterButton = Driver.FindElement( By.LinkText("Register") );
            mainPageRegisterButton.Click();

            // Text Fields
            var userNameField = Driver.FindElement( By.Id( "profile-user_login" ) );
            userNameField.SendKeys( "devusr_" + randomName );                        

            var emailField = Driver.FindElement( By.Id( "profile-email" ) );
            emailField.SendKeys( "development+devusr_" + randomName + "@weedseeker.net" );
            

            var passwordField = Driver.FindElement( By.Id( "profile-user_pass" ) );
            passwordField.SendKeys( "123456" );

            var confirmPasswordField = Driver.FindElement( By.Id( "profile-user_pass_confirm" ) );
            confirmPasswordField.SendKeys( "123456" );

            // Selects
            var country = Driver.FindElement( By.Id( "profile-country" ) );
            var countrySelectElement = new SelectElement( country );
            countrySelectElement.SelectByValue("224");

            var state = Driver.FindElement( By.Id( "profile-state" ) );
            var stateSelectElement = new SelectElement( state );
            stateSelectElement.SelectByValue( "1525" );

            var city = Driver.FindElement( By.Id( "profile-city" ) );
            var citySelectElement = new SelectElement( city );
            citySelectElement.SelectByValue( "5726" );

            // Checkbox
            var checkboxAgree = Driver.FindElement( By.Name( "agree" ) );
            checkboxAgree.Click();

            // Waits for all AngularJS requests to complete.
            Driver.WaitForAngularJsEventsToComplete();

            //Save Button
            var saveButton = Driver.FindElement( By.CssSelector( "button.btn-success" ) );
            saveButton.Click();

            var alertTitle = Driver.FindElement( By.CssSelector( ".alert-success h4" ) );
            
            Expect(alertTitle, Is.Not.Null);
            Expect(alertTitle.Text, Contains("Account created successfully"));
        }

        [Test]
        public void OpenLoginPageAndSignInTest()
        {
            OpenLoginPageAndSignIn("kllzn", "123456");

            var logoutButton = Driver.FindElement( By.LinkText( "Logout" ) );
            Assert.NotNull( logoutButton );
        }

        private string CreateRandomName()
        {
            var name = DateTime.Now.ToUniversalTime().Ticks.ToString("x");

            return name;
        }
    }
}
