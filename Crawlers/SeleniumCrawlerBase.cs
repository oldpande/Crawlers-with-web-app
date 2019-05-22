using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using CarBase.Business;
using NLog;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Firefox;
using System.Drawing;

namespace CarBase.Crawlers
{
    public abstract class SeleniumCrawlerBase : CrawlerBase, ICrawler
    {
        protected SeleniumCrawlerBase(Logger logManager) : base(logManager)
        {
        }

        protected void LoadDocument(RemoteWebDriver driver, string url)
        {
            int attempt = 60000;
            while (attempt < 3840000)
            {
                try
                {
                    driver.Navigate().GoToUrl(url);
                    break;
                }
                catch (WebDriverException ex)
                {
                    LogManager.Error(ex, $"\nPage load error. Url={url}. Error: {ex.Message}");
                    Thread.Sleep(attempt);
                    attempt *= 4;
                }
            }
        }

        protected IWebElement GetElement(IWebElement element, string id, string xpath, bool isRequired)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            if (id == null)
                throw new ArgumentNullException(nameof(id));
            if (xpath == null)
                throw new ArgumentNullException(nameof(xpath));

            if (IsElementPresent(By.XPath(xpath), element) == false)
            {
                if(isRequired)
                    throw new ExtractionException($"Required field {id} wasn't found in the page html.");
                return null;
            }

            return element.FindElement(By.XPath(xpath));
        }

        protected int GetPriceWithoutDot(IWebElement element, string id, string xpath, bool isRequired)
        {
            string priceString = "";
            IWebElement PriceNode = GetElement(element, $"GetPrise.{id}", xpath, isRequired);
            Regex rgx = new Regex(@"\d");
            foreach (Match match in rgx.Matches(PriceNode.Text))
                priceString += match.Value;
            return Convert.ToInt32(priceString);          
        }

        protected string GetDataString(IWebElement element, string xpathFindElement, string xpathGetElement)
        {
            element = element.FindElements(By.XPath(xpathFindElement))?.FirstOrDefault();
            if (element != null)
                return element.FindElement(By.XPath(xpathGetElement)).Text;

            return null;
        }

        protected double GetDataFloatWithoutDot(IWebElement element, string xpathFindElement, string xpathGetElement)
        {
            element = element.FindElements(By.XPath(xpathFindElement))?.FirstOrDefault();
            if (element != null)
                return Convert.ToDouble(element.FindElement(By.XPath(xpathGetElement)).Text.Replace(',', '.'));

            return 0;
        }

        protected double GetDataFloat(IWebElement element, string id, string xpath, bool isRequired)
        { 
            element = GetElement(element, id, xpath, isRequired);
            if (element != null)
                return Convert.ToDouble(element.Text.Replace(',', '.'));

            return 0;
        }

        protected bool IsElementPresent(By by, RemoteWebDriver driver)
        {
            try
            {
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        protected bool IsElementPresent(By by, IWebElement element)
        {
            try
            {
                element.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public List<Model> GetCars()
        {
            RemoteWebDriver driver = new FirefoxDriver();
            driver.Manage().Window.Size = new Size(1900, 1020);
            try
            {
                LoadDocument(driver, StartPageUrl);
                return GetCars(driver);
            }
            finally
            {
                driver.Close();
            }
        }

        protected abstract List<Model> GetCars(RemoteWebDriver driver);

        protected abstract string StartPageUrl { get; }
    }
}
