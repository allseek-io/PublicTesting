using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;

namespace WeedSeeker.Web.Test.Functional.Pages
{
    public abstract class Page
    {
        /// <summary>
        /// Creates a page for the specified webDriver instance and page.
        /// </summary>
        public static TPage Create<TPage>(IWebDriver webDriver) where TPage : Page
        {
            return (TPage)Activator.CreateInstance(typeof(TPage), webDriver);
        }

        protected Page(IWebDriver driver)
        {
            WebDriver = driver;
        }

        public IWebDriver WebDriver { get; set; }

        /// <summary>
        /// Base url
        /// </summary>
        protected string BaseUrl => $"http://alpha.weedseeker.net/";

        /// <summary>
        /// BaseUrl + Path. Used by <see cref="Navigate(string)"/>.
        /// </summary>
        protected virtual string Url => BaseUrl + Path;

        /// <summary>
        /// Current address without <see cref="BaseUrl"/>.
        /// </summary>
        public virtual string CurrentAddress => WebDriver.Url.Replace(BaseUrl, "").ToLower();

        /// <summary>
        /// This page's path. Used by <see cref="Navigate(string)"/>.
        /// </summary>
        public abstract string Path { get; }

        /// <summary>
        /// Reloads the current page.
        /// </summary>
        public virtual void Reload() => WebDriver.Navigate().Refresh();

        /// <summary>
        /// The body web element from the current page.
        /// </summary>
        public virtual IWebElement Body => GetElementByCSSSelector("body");

        /// <summary>
        /// Navigates to the specified path.
        /// </summary>
        public virtual void Navigate(string part)
        {
            if (!string.IsNullOrWhiteSpace(part) && part.StartsWith("/"))
                WebDriver.Navigate().GoToUrl(BaseUrl + part);
            else
                Navigate(new string[] { part });
        }

        /// <summary>
        /// Navigates to the specified Url, interpolating parts with the specified parts.
        /// </summary>
        /// <param name="parts"></param>
        public virtual void Navigate(params string[] parts)
        {
            if (Url.Contains("{") && Url.Substring(Url.IndexOf("{") + 2, 1) == "}")
            {
                var urlStringFormat = string.Format(Url, parts);
                WebDriver.Navigate().GoToUrl(urlStringFormat);
                return;
            }
            var url = parts.Aggregate(Url, (current, part) => current + ("/" + part));
            WebDriver.Navigate().GoToUrl(url);
        }

        /// <summary>
        /// Try to execute function (usually involving getting some element), and
        /// if a <see cref="StaleElementReferenceException"/> occurs, retries up
        /// to the specified number of times.
        /// </summary>
        public virtual T ProtectFromStaleReference<T>(Func<T> func, int times = 5)
        {
            var i = 0;
            StaleElementReferenceException exception = null;
            while (i++ < times)
            {
                try
                {
                    return func();
                }
                catch (StaleElementReferenceException ex)
                {
                    exception = ex;
                    Thread.Sleep(200);
                }
            }
            if (exception == null) throw new ApplicationException($"Tried to get value {times} times, no success.");
            throw new ApplicationException("Couldn't get value, got a StaleElementReferenceException.", exception);
        }

        /// <summary>
        /// Waits until the current address matches the specified address.
        /// </summary>
        public virtual async Task<bool> WaitForCurrentAddressAsync(string address)
        {
            return await WaitAsync(driver => string.Compare(driver.Url.Replace(BaseUrl, ""), address, true) == 0);
        }

        /// <summary>
        /// Wait until <c>window.$.active === 0</c> is <c>true</c>.
        /// </summary>
        public virtual async Task WaitAjaxAsync()
        {
            await WaitForJQueryToBeLoadedAsync(20000, true);
            await WaitAsync(driver => Convert.ToBoolean(((IJavaScriptExecutor)WebDriver).ExecuteScript("return typeof(window.$) === 'undefined' ? false : window.$.active === 0")));
        }

        /// <summary>
        /// Wait until jquery is loaded.
        /// </summary>
        public virtual Task<bool> WaitForJQueryToBeLoadedAsync(int milliTimeout, bool shouldThrow = false)
        {
            Func<Task<bool>> wait = () => WaitAsync(driver => Convert.ToBoolean(((IJavaScriptExecutor)WebDriver).ExecuteScript("return typeof(window.$) === 'function'")));
            return WaitAsync(wait, milliTimeout, shouldThrow, "jQuery not found.");
        }

        /// <summary>
        /// Wait for element to be clickable (in Selenium, <c>Displayed</c> and <c>Enabled</c>).
        /// If not found may throw if requested.
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="shouldThrow"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public virtual async Task<bool> WaitForClickableElementAsync(string selector, bool shouldThrow = false, int seconds = 5)
        {
            var wasFound = await WaitAsync(driver =>
            {
                var elements = WebDriver.FindElements(By.CssSelector(selector));
                if (elements.Count == 0) return false;
                var element = elements[0];
                return element.Displayed && element.Enabled;
            }, seconds);
            if (wasFound) return true;
            if (shouldThrow) throw new ApplicationException($"Element {selector} not found or not clickable.");
            return false;
        }

        /// <summary>
        /// Checks if there is a listener attached to an object using jquery by verifying
        /// <c>Object.keys(window.$._data($('{selector}')[0], 'events')</c>.
        /// </summary>
        public virtual async Task<bool> WaitElementToHaveAListenersAsync(string selector, bool shouldThrow = false)
        {
            await WaitForJQueryToBeLoadedAsync(20000, true);
            var isLoaded = await WaitAsync(driver =>
            {
                try
                {
                    var hasSelectors = Convert.ToBoolean(((IJavaScriptExecutor)WebDriver).ExecuteScript($"return typeof(window.$) === 'undefined' ? false : Object.keys(window.$._data($('{selector}')[0], 'events')).length > 0;"));
                    return hasSelectors;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error waiting for an element to have a listener: " + ex.ToString());
                    return false;
                }
            });
            if (isLoaded) return true;
            if (shouldThrow) throw new ApplicationException($"Element {selector} does not have an attached listener.");
            return false;
        }

        /// <summary>
        /// Wait for a function to return true, using a <see cref="IWebDriver"/> as a parameter.
        /// </summary>
        public virtual async Task<bool> WaitAsync(Func<IWebDriver, bool> fn, int segundos = 5)
        {
            await Task.Yield();
            var wait = new WebDriverWait(WebDriver, TimeSpan.FromSeconds(segundos));
            try
            {
                return wait.Until(fn);
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }

        /// <summary>
        /// Wait until the specified element has items.
        /// </summary>
        public async Task<bool> WaitSelectToLoadItensWithAjaxAsync(SelectElement select)
        {
            await Task.Yield();
            for (int i = 0; i < 5; i++)
            {
                Thread.Sleep(1000);
                try
                {
                    if (select.Options.Where(x => x.GetAttribute("value") != "0" && x.GetAttribute("value") != "").Count() > 0) return true;
                }
                catch { }
            }
            return false;
        }

        /// <summary>
        /// Takes a screenshot and saves to the specified file.
        /// </summary>
        public virtual void TakeScreenshoot(string fileName = null)
        {
            var screenShoot = WebDriver.TakeScreenshot();
            screenShoot.SaveAsFile(fileName, ImageFormat.Png);
        }

        protected virtual IWebElement GetElementByCSSSelector(string selector) => GetElement(By.CssSelector(selector));

        protected virtual IWebElement GetElement(By by)
        {
            var wait = new WebDriverWait(WebDriver, TimeSpan.FromSeconds(30));
            IWebElement element = null;
            try
            {
                if (wait.Until(x => (element = WebDriver.FindElement(by)).Displayed && element.Enabled)) return element;
            }
            catch
            {
                return null;
            }
            return null;
        }

        protected virtual List<IWebElement> GetElementsByCSSSelector(string selector)
        {
            Func<List<IWebElement>> getElements = () =>
            {
                try
                {
                    return WebDriver.FindElements(By.CssSelector(selector)).ToList();
                }
                catch (NoSuchElementException)
                {
                    WebDriver.Navigate().Refresh();
                    return WebDriver.FindElements(By.CssSelector(selector)).ToList();
                }
            };

            var wait = new WebDriverWait(WebDriver, TimeSpan.FromSeconds(20));
            wait.Until(x => getElements().Any());

            return getElements();
        }

        protected virtual bool IsVisible(string selector)
        {
            var els = WebDriver.FindElements(By.CssSelector(selector));
            if (els.Count == 0) return false;
            var el = els[0];
            return el.Displayed;
        }

        protected virtual void Click(string selector) => WebDriver.FindElement(By.CssSelector(selector)).Click();

        /// <summary>
        /// Check if angular is doing something and waits
        /// </summary>
        public virtual async Task WaitAngularAsync()
        {
            await WaitAsync(driver => Convert.ToBoolean(((IJavaScriptExecutor)WebDriver).ExecuteScript(@"
try {
  if (document.readyState !== 'complete') {
    return false; // Page not loaded yet
  }
  if (window.angular) {
    if (!window.qa) {
      // Used to track the render cycle finish after loading is complete
      window.qa = { doneRendering: false };
    }
    // Get the angular injector for this app (change element if necessary)
    var injector = window.angular.element('body').injector();
    // Store providers to use for these checks
    var $rootScope = injector.get('$rootScope');
    var $http = injector.get('$http');
    var $timeout = injector.get('$timeout');
    // Check if digest
    if ($rootScope.$$phase === '$apply' || $rootScope.$$phase === '$digest' || $http.pendingRequests.length !== 0) {
      window.qa.doneRendering = false;
      return false; // Angular digesting or loading data
    }
    if (!window.qa.doneRendering) {
      // Set timeout to mark angular rendering as finished
      $timeout(function() {
        window.qa.doneRendering = true;
      });
      return false;
    }
  }
  if (window.jQuery) {
    if (window.jQuery.active) {
      return false;
    } else if (window.jQuery.ajax && window.jQuery.ajax.active) {
      return false;
    }
  }
  if (window.require) {
    var $ = window.require('jquery');
    if ($) {
      if ($.active) {
        return false;
      } else if ($.ajax && $.ajax.active) {
        return false;
      }
    }
  }
  return true;
} catch (ex) {
  return false;
}
")));
        }

        private async Task<bool> WaitAsync(Func<Task<bool>> task, int milli, bool shouldThrow = false, string waitErrorMessage = "Wait condition not met.")
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var t = WaitAsync(task, cancellationTokenSource.Token);
            await Task.WhenAny(t, Task.Delay(milli));
            var taskCompleted = t.IsCompleted;
            if (!taskCompleted) cancellationTokenSource.Cancel();
            if (await t) return true;
            if (shouldThrow) throw new ApplicationException(waitErrorMessage);
            return false;
        }

        private async Task<bool> WaitAsync(Func<Task<bool>> task, CancellationToken token)
        {
            do
            {
                var finished = await task();
                if (finished) return true;
                if (token.IsCancellationRequested) return false;
            } while (true);
        }

        /// <summary>
        /// Gets value from input.
        /// </summary>
        public virtual string GetInputValue(string cssSelector) => GetElementByCSSSelector(cssSelector).GetAttribute("value");

        /// <summary>
        /// Types value in input.
        /// </summary>
        public virtual void TypeInputValue(string cssSelector, string text) => GetElementByCSSSelector(cssSelector).SendKeys(text);

        /// <summary>
        /// Gets text from selector.
        /// </summary>
        public virtual string LabelValue(string cssSelector) => GetElementByCSSSelector(cssSelector).Text;

        /// <summary>
        /// Checks if element is <see cref="IWebElement.Displayed"/>.
        /// </summary>
        public virtual bool IsDisplayed(string objectId)
        {
            try
            {
                return GetElement(By.Id(objectId)).Displayed;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Selects an item on a &lt;select&gt; by index.
        /// </summary>
        public virtual void SelectElementInDropdown(int index, string selector)
        {
            var element = GetElement(By.CssSelector(selector));
            var selectElement = new SelectElement(element);
            selectElement.SelectByIndex(index);
        }

        /// <summary>
        /// Selects an item on a &lt;select&gt; by text.
        /// </summary>
        public virtual void SelectElementInDropDown(string text, string selector)
        {
            var element = GetElement(By.CssSelector(selector));
            var selectElement = new SelectElement(element);
            selectElement.SelectByText(text);
        }
    }
}
