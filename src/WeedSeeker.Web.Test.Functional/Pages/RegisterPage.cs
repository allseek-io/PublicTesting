using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace WeedSeeker.Web.Test.Functional.Pages
{
    public class RegisterPage : Page
    {
        public RegisterPage(IWebDriver driver) : base(driver)
        {
        }

        public override string Path => "/join";

        public string Username
        {
            get
            {
                return GetInputValue("#profile-user_login");
            }
            set
            {
                TypeInputValue("#profile-user_login", value);
            }
        }

        public string Email
        {
            get
            {
                return GetInputValue("#profile-email");
            }
            set
            {
                TypeInputValue("#profile-email", value);
            }
        }

        public string Password
        {
            get
            {
                return GetInputValue("#profile-user_pass");
            }
            set
            {
                TypeInputValue("#profile-user_pass", value);
            }
        }

        public string ConfirmPassword
        {
            get
            {
                return GetInputValue("#profile-user_pass_confirm");
            }
            set
            {
                TypeInputValue("#profile-user_pass_confirm", value);
            }
        }

        public string Country
        {
            get
            {
                return GetInputValue("#profile-country");
            }
            set
            {
                SelectElementInDropDown(value, "#profile-country");
            }
        }

        public string State
        {
            get
            {
                return GetInputValue("#profile-state");
            }
            set
            {
                SelectElementInDropDown(value, "#profile-state");
            }
        }

        public string City
        {
            get
            {
                return GetInputValue("#profile-city");
            }
            set
            {
                SelectElementInDropDown(value, "#profile-city");
            }
        }

        public void Submit()
        {
            var button = GetElementByCSSSelector("button.btn-success");
            button.Click();
        }

        /// <summary>
        /// This item does not belongs to the registration page, but for the
        /// sake of simplicity we are going to use on the RegisterPage object.
        /// </summary>
        public string ConfirmationLabel => LabelValue("#error_message");

        public IWebElement Agree => GetElement(By.Name("agree"));
    }
}
