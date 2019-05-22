using CarBase.Business;
using HtmlAgilityPack;
using NLog;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CarBase.Crawlers
{
    public class VolkswagenByCrawler : HtmlAgilityCrawlerBase
    {
        public VolkswagenByCrawler(Logger logManager) : base(logManager)
        {
        }

        protected override string Domain => "https://www.volkswagen.by";

        protected override string CatalogUrl => "/models/";

        private HtmlDocument MainModelDocument;

        protected override List<Model> GetCars(HtmlDocument document)
        {
            var carModelsList = new List<Model>();
            try
            {
                foreach (var node in document.DocumentNode.SelectNodes(@"//div[@class='models']/div[position()<3]//div[@class='models__item']|
                                                                         //div[@class='models']/div[position()<3]//div[@class='models__item-wrapper']"))
                {
                    if (node.SelectSingleNode("./a").Attributes["href"].Value == "#")
                        continue;

                    string carModelsUrl = ExtractLink(node, "Car.Url", ".//a[@class='models__title']|.//a[@class='models-view__title']", true);

                    HtmlDocument documentModels = LoadHtmlDocument(carModelsUrl);
                    var carModelSector = ExtractNodeCollection(documentModels.DocumentNode, "//div[@class='complectation__item']");
                    if (carModelSector == null)
                        continue;
                    MainModelDocument = documentModels;
                    foreach (var carModelSectorNode in carModelSector)
                        carModelsList.Add(GetCarModel(carModelSectorNode, ".//h4", ".//p", Domain+carModelsUrl));
                }
            }
            catch (ExtractionException ex)
            {
                LogManager.Fatal(ex, "Car parsing error");
                throw;
            }

            return carModelsList;
        }

        private Model GetCarModel(HtmlNode node, string xpathName, string xpathPrice, string url)
        {
            var carModel = new Model();

            carModel.Name = GetModelName(node, "Car.Name", xpathName, true);
            carModel.PriceFrom = ExtractPrice(node, "Car.Price", xpathPrice, true);
            carModel.Versions = GetCarVersions(node);
            carModel.Link = url;
            carModel.BrandId = 3;
            LogCar(carModel);
            return carModel;
        }

        private string GetModelName(HtmlNode node, string id, string xpathName, bool isRequired)
        {
            var name = ExtractText(MainModelDocument.DocumentNode, "CarModel.MainName", "//div[@class='model-top__head']/h4", true);
            name += "-" + ExtractText(node, id, xpathName, isRequired);
            return name;
        }

        private List<CarVersion> GetCarVersions(HtmlNode carNode)
        {
            var availableVersions = new List<CarVersion>();
            int numberVersionPrice = ExtractPrice(carNode, "CarVersion.Price", ".//p", false);

            var nodeCollection = ExtractNodeCollection(carNode, ".//div[@class='accordion-chars__info dnone' or @class='accordion-chars__info']");
            if (nodeCollection == null)
                return null;
            foreach (var nodeVersion in nodeCollection)
                availableVersions.Add(GetCarVersion(nodeVersion, numberVersionPrice));

            return availableVersions;
        }

        private CarVersion GetCarVersion(HtmlNode nodeVersion, int numberVersionPrice)
        {
            var carVersion = new CarVersion();
            carVersion.Drive = "Механика / Автомат";
            carVersion.Engine = null;
            carVersion.EnvironmentalStandard = ExtractText(
                nodeVersion, "CarVersion.EnvironmentalStandard",
                ".//div[@class='accordion-chars__item'][4]/div/div[2]", false);

            carVersion.FuelType = "Бензин";
            carVersion.FuelConsumptionAverage = GetFuelAverage(nodeVersion);
            carVersion.Price = numberVersionPrice;
            carVersion.Transmission = null;
            carVersion.FuelConsumptionInCity = null;
            carVersion.FuelConsumptionInCity = null;
            return carVersion;
        }

        private double? GetFuelAverage(HtmlNode nodeVersion)
        {
            Regex rgx = new Regex(@"\d\.\d");
            string fuelConsumptionAverage = ExtractText(
                nodeVersion, "CarVersion.FuelConsumptionAverage", 
                ".//div[@class='accordion-chars__item'][3]/div/div[2]", false).Replace(",",".");
            if (fuelConsumptionAverage == "")
                return null;
            return Convert.ToDouble(rgx.Match(fuelConsumptionAverage).Value);
        }
    }
}
