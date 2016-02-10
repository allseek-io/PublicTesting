using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework.Internal;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;

namespace WeedSeeker.Web.Test.Functional
{
    public static class SeleniumWebDriverExtensions
    {
        /// <summary>
        /// Waits for the AngularJS events to complete.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="timeout">The timeout (in milliseconds).</param>
        public static void WaitForAngularJsEventsToComplete(this IWebDriver driver, int timeout = 3000)
        {
            driver.WaitForAngularJsEventsToComplete(TimeSpan.FromMilliseconds(timeout));
        }

        /// <summary>
        /// Waits for the AngularJS events to complete.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="timeout">The timeout.</param>
        public static void WaitForAngularJsEventsToComplete(this IWebDriver driver, TimeSpan timeout)
        {
            // This is an ugly, shameful, workaround. 
            // Need to find a better solution for that.
            new WebDriverWait(driver, timeout).Until(
                d =>
                    d.ExecuteJavaScript<bool>(
                        "return (window.angular !== undefined) && (angular.element(document).injector() !== undefined) && (angular.element(document).injector().get('$http').pendingRequests.length === 0)"));
        }

        /// <summary>
        /// Forces the driver to util the specified element exists
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="elementSelection">The element selection.</param>
        public static void WaitForElement(this IWebDriver driver, By elementSelection)
        {
            driver.WaitForElement(elementSelection, TimeSpan.FromSeconds(30));
        }

        /// <summary>
        /// Forces the driver to util the specified element exists
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="elementSelection">The element selection.</param>
        /// <param name="timeout">The timeout.</param>
        public static void WaitForElement(this IWebDriver driver, By elementSelection, TimeSpan timeout)
        {
            new WebDriverWait(driver, timeout).Until(d => d.FindElement(elementSelection));
        }

        /// <summary>
        /// Waits until the specified condition is true.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="driver">The driver.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="timeout">The timeout.</param>
        public static void WaitUntil<TResult>(this IWebDriver driver, Func<IWebDriver, TResult> condition,
            TimeSpan timeout)
        {
            new WebDriverWait(driver, timeout).Until(condition);
        }

        /// <summary>
        /// Checks whether an element exists
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="elementSelection">The element selection.</param>
        /// <returns>A <see cref="bool"/> indicating whether the specified element exists.</returns>
        public static bool ElementExists(this IWebDriver driver, By elementSelection)
        {
            var result = false;
            try
            {
                driver.FindElement(elementSelection);
                result = true;
            }
            catch (NoSuchElementException ex) when (Log($"Element not Found: {ex.Message}")){}

            return result;

        }


        /// <summary>
        /// Finds an element or returns its default value if not found
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="elementSelection">The element selection.</param>
        /// <returns>A <see cref="bool"/> indicating whether the specified element exists.</returns>
        public static IWebElement FindElementOrDefault(this IWebDriver driver, By elementSelection)
        {
            var result = default(IWebElement);
            try
            {
                result = driver.FindElement(elementSelection);
            }
            catch (NoSuchElementException ex) when (Log($"Element not Found: {ex.Message}")) {}

            return result;

        }

        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>System.Boolean.</returns>
        private static bool Log(string message)
        {
            var logger = new Logger(TestExecutionContext.CurrentContext.CurrentTest.MethodName, InternalTraceLevel.Info, Console.Out);

            logger.Info(message);

            return false;
        }
    }
}
