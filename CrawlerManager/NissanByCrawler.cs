using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using NLog;
using CarBase.Business;

namespace CarBase.CrawlerManager
{
    public class NissanByCrawler : SeleniumCrawlerBase
    {
        public NissanByCrawler(Logger logManager) : base(logManager)
        {
        }

        protected override string StartPageUrl => "https://www.nissan.by/vehicles/";

        private void OpenAllSections(RemoteWebDriver driver)
        {
            foreach (var section in driver.FindElements(By.XPath("//div[@class='section']")))
            {
                section.Click();
            }
        }

        protected override List<Model> GetCars(RemoteWebDriver driver)
        {
            List<string> carModelLinks = GetCarModelLinks(driver);

            var cars = new List<Model>();

            foreach (var link in carModelLinks)
            {
                // load car page
                LoadDocument(driver, link);

                // open all sections on the page
                OpenAllSections(driver);

                int sectionIndex = 1;
                foreach(IWebElement element in driver.FindElements(By.XPath("//div[@class='automobile cols3 wrapper']")))
                {
                    var car = GetCarModel(element, sectionIndex, link, GetTitleName(driver));
                    //1 car has 3 sections
                    cars.Add(car);
                    sectionIndex += 3;
                }
            }
            return cars;
        }

        private string GetTitleName(RemoteWebDriver driver)
        {
            string titleName = driver.FindElementByXPath("//div[@class='title']").Text;

            return titleName;
        }

        private List<string> GetCarModelLinks(RemoteWebDriver driver)
        {
            var links = new List<string>();
            foreach (IWebElement element in driver.FindElements(By.XPath("//ul//h3/a")))
            {
                string link = element.GetAttribute("href") + "price-specifications/";
                links.Add(link);
            }
            return links;
        }

        private Model GetCarModel(IWebElement element, int IndexSection, string link, string titleName)
        {
            var carModel = new Model();

            carModel.Name = titleName + " - " + element.FindElement(By.XPath(".//h2")).Text;

            carModel.PriceFrom = GetPriceWithoutDot(element, "Model.PriceFrom", ".//div[@class='price-cost']", true);

            carModel.Versions = GetCarVersions(element,
                $"//div[@class='section active'][{IndexSection}]//table",
                $"//div[@class='section active'][{IndexSection + 2}]");

            carModel.Link = link;

            carModel.BrandId = 2;

            LogCar(carModel);
            return carModel;
        }

        private List<CarVersion> GetCarVersions(IWebElement element, string XpathVersions, string XpathSpecific)
        {
            var ListData = new List<CarVersion>();
            for (int NumberVersion = 2; NumberVersion <= element.FindElements(By.XPath(XpathVersions)).Count - 1; NumberVersion++)
            {
                try
                {
                    var VersionData = new CarVersion();
                    VersionData.Engine = element.FindElement(By.XPath(XpathVersions + $"//tr[{NumberVersion}]/td[1]")).Text;
                    VersionData.Drive = element.FindElement(By.XPath(XpathVersions + $"//tr[{NumberVersion}]/td[2]")).Text;
                    VersionData.Transmission = element.FindElement(By.XPath(XpathVersions + $"//tr[{NumberVersion}]/td[3]")).Text;
                    VersionData.Price = GetPriceWithoutDot(element, "Version.Price", XpathVersions + $"//tr[{NumberVersion}]/td[4]", false);
                    VersionData.FuelType = GetDataString(element, XpathSpecific + "//tr[./td[contains(text(),'бака')]]", "./td[3]");
                    VersionData.EnvironmentalStandard = GetDataString(element, XpathSpecific + "//tr[./td[contains(text(),'Экологический')]]", "./td[3]");
                    VersionData.FuelConsumptionInCity = GetDataFloatWithoutDot(element, XpathSpecific + "//tr[./td[contains(text(),'городском')]]", "./td[3]");
                    VersionData.FuelConsumptionOutCity = GetDataFloatWithoutDot(element, XpathSpecific + "//tr[./td[contains(text(),'загородном')]]", "./td[3]");
                    VersionData.FuelConsumptionAverage = GetDataFloatWithoutDot(element, XpathSpecific + "//tr[./td[contains(text(),'комбинированном')]]", "./td[3]");

                    ListData.Add(VersionData);
                }
                catch (WebDriverException ex)
                {
                    LogManager.Info(ex, "\nException: " + ex.Message);
                    throw;
                }
            }
            return ListData;
        }
    }
}