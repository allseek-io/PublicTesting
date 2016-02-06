using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace WeedSeeker.Web.Test.Functional
{
    public static class SeleniumWebDriverExtensions
    {
        /// <summary>
        /// Waits for the AngularJS model calculations to finish.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="timeout">The timeout (in milliseconds).</param>
        public static void WaitForAngularJsModelBinding(this IWebDriver driver, int timeout = 3000)
        {
            driver.WaitForAngularJsModelBinding(TimeSpan.FromMilliseconds(timeout));
        }

        /// <summary>
        /// Waits for the AngularJS model calculations to finish.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="timeout">The timeout.</param>
        public static void WaitForAngularJsModelBinding(this IWebDriver driver, TimeSpan timeout)
        {
            // This is an ugly, shameful, workaround. 
            // Need to find a better solution for that.
            Thread.Sleep(timeout);            
        }
    }
}
