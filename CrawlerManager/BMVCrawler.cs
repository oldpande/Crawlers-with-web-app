using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarBase.Business;
using NLog;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace CarBase.CrawlerManager
{
    class BMVCrawler : HtmlAgilityCrawlerBase
    {
        public BMVCrawler(Logger logManager) : base(logManager)
        {
        }

        protected override string CatalogUrl => "/ru/all-models.html";

        protected override string Domain => "https://www.bmw.by";

        protected override List<Model> GetCars(HtmlDocument document)
        {
            var cars = new List<Model>();
            foreach (var nodeRow in ExtractNodeCollection(document.DocumentNode, "//div[@class='car-series'][position()<12]"))
                foreach (var nodeModel in ExtractNodeCollection(nodeRow, "./div[@class='cars']/div"))
                {
                    var carModel = new Model();
                    carModel.Name = ExtractText(nodeModel, "CarModel.Name", ".//h4", true);
                    if (carModel.Name == "BMW 3 серии Седан" || carModel.Name== "BMW M760Li xDrive Седан")
                        continue;
                    //no one can't be double
                    var names = cars.Where(c => c.Name == carModel.Name);
                    if (names.Count() != 0)
                        continue;

                    carModel.BrandId = 4;
                    carModel.PriceFrom = null;
                    carModel.Link = СonstructLink(ExtractLink(nodeModel, "CarModel.Link", ".//div[@class='ds2-model-card--image']/a", true), carModel.Name);
                    carModel.Versions = GetVersions(carModel.Link);
                    carModel.Link = Domain + carModel.Link;
                    LogCar(carModel);
                    cars.Add(carModel);
                }
            return cars;
        }

        private string СonstructLink(string link, string name)
        {
            string partOfLink = "";
            switch (name)
            {
                case "BMW 3 серии Седан": partOfLink = "techincal-data.html#tab-0"; break;
                case "BMW M760Li xDrive": partOfLink = "techincal-data.html#tab-0"; break;
                case "BMW 7 серии Седан": partOfLink = "bmw-7-series-inform.html"; break;
                case "BMW 8 серии Купе": partOfLink = "the-8-technical-data.html#tab-0"; break;
                case "BMW X7": partOfLink = "bmw-x7-inform.html"; break;
                case "BMW Z4 Родстер": partOfLink = "bmw-z4-roadster-inform.html"; break;
                default: partOfLink = "technical-data.html#tab-0"; break;
            }
                
            string[] words = link.Split('/');
            words[words.Length - 1] = partOfLink;
            link = "";
            foreach (var s in words)
                link += s + '/';
            return link;
        }

        private List<CarVersion> GetVersions(string link)
        {
            var document = LoadHtmlDocument(link);
            var listVersions = new List<CarVersion>();
            foreach (var node in ExtractNodeCollection(document.DocumentNode, "//div[@class='ds2-mvc-tabs-content']/section"))
                listVersions.Add(GetOneVersion(node));

            return listVersions;
        }

        private CarVersion GetOneVersion(HtmlNode node)
        {
            var carVersion = new CarVersion();
            carVersion.Engine = ExtractText(node, "CarVersion.Engine", ".//div[@class='row']/div[2]//tbody/tr[2]/td[2]", false).Replace(" ", string.Empty).Replace("\n", string.Empty);
            carVersion.Drive = null;
            carVersion.EnvironmentalStandard = ExtractText(node, "carVersion.EnvironmentStandart", ".//div[@class='row']/div[4]//tbody/tr[4]/td[2]", false).Replace(" ", string.Empty).Replace("\n", string.Empty);
            carVersion.FuelConsumptionAverage = ExtractDouble(node, "carVersion.FuelConsumptionAverage", ".//div[@class='row']/div[4]//tbody/tr[3]/td[2]", false);
            carVersion.FuelConsumptionOutCity = ExtractDouble(node, "carVersion.FuelConsumptionOutCity", ".//div[@class='row']/div[4]//tbody/tr[2]/td[2]", false);
            carVersion.FuelConsumptionInCity = ExtractDouble(node, "carVersion.FuelConsumptionInCity", ".//div[@class='row']/div[4]//tbody/tr[1]/td[2]", false);
            carVersion.FuelType = null;
            carVersion.Price = null;
            carVersion.Transmission = null;
            return carVersion;
        }

        private double? ExtractDouble(HtmlNode node, string id, string xpath, bool isRequired)
        {
            string doubleNumber = ExtractText(node, id, xpath, isRequired).Replace(" ", string.Empty).Replace("\n", string.Empty);
            if (doubleNumber == null || doubleNumber == "n/a" || doubleNumber == "н/п*" || doubleNumber == "н/п" || doubleNumber == "" || doubleNumber == "-")
                return null;
            doubleNumber = doubleNumber.Replace('.', ',');
            return double.Parse(Regex.Matches(doubleNumber, @"\d+\,\d+")[0].Value);
        }
    }
}
