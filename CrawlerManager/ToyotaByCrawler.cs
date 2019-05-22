using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using NLog;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;

namespace CarBase.CrawlerManager
{
    public class ToyotaByCrawler : SeleniumCrawlerBase
    {
        private string FirstName;

        public ToyotaByCrawler(Logger logManager) : base(logManager)
        {
        }

        protected override string StartPageUrl => "https://toyota.by/";
        
        protected override List<Model> GetCars(RemoteWebDriver driver)
        {
            List<Model> cars = new List<Model>();

            for (int i = 1; i < driver.FindElementsByXPath("//div[@class='card-grid__col']").Count; i++)
            {
                var element = driver.FindElementByXPath($"//div[@class='card-grid__col'][{i}]");
                FirstName = element.FindElement(By.XPath(".//a[@class='car-card__title-link']")).Text;
                cars = FindAndGetCars(driver, element);
            }
            return cars;
        }

        private List<Model> FindAndGetCars(RemoteWebDriver driver, IWebElement element)
        {
            element.Click();
            element = driver.FindElementByXPath("//li[@class='t-menu-links__item'][2]");
            element.Click();
            Thread.Sleep(6000);
            var carModelsList = new List<Model>();
            foreach (var tab in driver.FindElementsByXPath("//div[@class='slider-tabs slider-tabs_fake']/slick/div[@class='slick-list draggable']/div[@class='slick-track']/div"))
            {
                var carModel = new Model();
                // Need check if arrow of list models is present 
                if (tab.GetAttribute("tabindex") == "-1")
                    driver.FindElement(By.XPath("//div[@class='slider-tabs slider-tabs_fake']/slick/button[@class='carousel__arrow slick-next slick-arrow']")).Click();
                
                carModel.Name = FirstName + " " + GetElement(tab, "CarModel.Name", "./button/div[1]", true).Text;
                carModel.PriceFrom = GetPriceWithoutDot(tab, "Model.PriceFrom", "./button//span[@class='ng-binding']", true);
                tab.Click();
                carModel.BrandId = 5;
                carModel.Link = driver.Url;
                carModel.Versions = GetAvailableVersion(driver);
                LogCar(carModel);
                carModelsList.Add(carModel);
            }

            driver.Navigate().Back();
            driver.Navigate().Back();
            return carModelsList;
        }
                
        private List<CarVersion> GetAvailableVersion(RemoteWebDriver driver)
        {
            var availableVersions = new List<CarVersion>();
            foreach (var row in driver.FindElementsByXPath("//table[@class='spec-table__self']/tbody/tr"))
            {
                var carVersion = new CarVersion();
                carVersion.Drive = GetElement(row, "CarVersion.Drive.Top", ".//td[3]//div[@class='stat-teaser__title ng-binding']", false).Text + " " +
                                   GetElement(row, "CarVersion.Drive.Button", ".//td[3]//div[@class='stat-teaser__subtitle ng-binding']", false).Text;
                carVersion.Engine = GetElement(row, "CarVersion.Engine.Top", ".//td[1]//div[@class='stat-teaser__title ng-binding']", false).Text + " " +
                                    GetOnePartOfString(GetElement(row, "CarVersion.Engine.Button", ".//td[1]//div[@class='stat-teaser__subtitle ng-binding']", false).Text, 1);//GetElement(row, "CarVersion.Engine.Button", ".//td[1]//div[@class='stat-teaser__subtitle ng-binding']", false).Text.Remove(0, 13);
                carVersion.EnvironmentalStandard = null;
                carVersion.FuelConsumptionAverage = GetDataFloat(row, "CarVersion.FuelConsumptionAverage", ".//td[4]/div/span/div/div/div[@class='stat-teaser__title ng-binding']", false);
                carVersion.FuelConsumptionInCity = null;
                carVersion.FuelConsumptionOutCity = null;
                carVersion.FuelType = GetOnePartOfString(GetElement(row, "CarVersion.FuelType", ".//td[1]//div[@class='stat-teaser__subtitle ng-binding']", false).Text, 0);
                carVersion.Price = GetPriceWithoutDot(row, "Price", ".//td[6]//span[@class='ng-binding']", false);
                carVersion.Transmission = GetElement(row, "CarVersion.Transmission.Top", ".//td[2]//div[@class='stat-teaser__title ng-binding']", false).Text + " " +
                                          GetElement(row, "CarVersion.Transmission.Button", ".//td[2]//div[@class='stat-teaser__subtitle']/ng-pluralize", false).Text;
                availableVersions.Add(carVersion);
            }
            return availableVersions;
        }

        private string GetOnePartOfString(string text, int part)
        {
            String[] words = text.Split(',');
            return words[part].Replace("?", "").Replace("\r\n", "");
        }
    }
}