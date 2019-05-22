using System;
using System.Collections.Generic;
using System.Threading;
using CarBase.Business;
using HtmlAgilityPack;
using NLog;

namespace CarBase.Crawlers
{
    public class EliteAutoByCrawler : HtmlAgilityCrawlerBase
    {
        public EliteAutoByCrawler(Logger logManager) : base(logManager)
        {
        }

        protected override string Domain => "http://cars.eliteauto.by";

        protected override string CatalogUrl => "/catalog/volvo";

        protected override List<Model> GetCars(HtmlDocument document)
        {
            var carModelsList = new List<Model>();
            try
            {
                foreach (var node in ExtractNodeCollection(document.DocumentNode, "//div[@class='row model-list']/div[@class='col-xs-24 col-sm-12 col-md-8 ']"))
                    carModelsList.Add(GetCarModel(node, ".//div[@class='m3']/span[@class='model_name']", ".//div[@class='price m3']/span"));
            }
            catch (ExtractionException ex)
            {
                LogManager.Fatal(ex, "Car parsing error");
                throw;
            }
            return carModelsList;
        }

        private Model GetCarModel(HtmlNode node, string xpathName, string xpathPrice)
        {
            var model = new Model();

            model.Name = ExtractText(node, "Car.Name", xpathName, true);
            model.PriceFrom = ExtractPrice(node, "Car.Price", xpathPrice, true);
            // 1. Get link of car model
            string carModelUrl = ExtractLink(node, "Car.Url", ".//div[@class='m10']/a", true);
            model.Versions = GetCarVersions(carModelUrl);
            model.Link = Domain + carModelUrl;
            model.BrandId = 1;
            LogCar(model);
            return model;
        }

        private List<CarVersion> GetCarVersions(string carModelUrl)
        {
            // 2. Load car model page
            HtmlDocument document = LoadHtmlDocument(carModelUrl);

            // 3. Get car version from the page
            var availableVersions = new List<CarVersion>();
            foreach (var carVersionNode in ExtractNodeCollection(document.DocumentNode, "//div[@class='flex-model-content-row new-vehicle-list']/div"))
            {
                CarVersion carVersion = GetCarVersion(carVersionNode);
                availableVersions.Add(carVersion);
            }
            return availableVersions;
        }

        private CarVersion GetCarVersion(HtmlNode node)
        {
            var carVersion = new CarVersion();

            // 1. Extract car version data from car model document
            carVersion.Price = ExtractPrice(node, "CarVersion.Price", ".//div[@class='price']/span[1]", false);

            // 2. Get car version url
            string carVersionUrl = ExtractLink(node, "CarVersion.Url", ".//a[@class='flex-model-card-title']", true);

            // 3. Load car version page
            HtmlDocument carVersionDocument = LoadHtmlDocument(carVersionUrl);

            // 4. Extract car version data from the page
            ExtractCarVersion(carVersionDocument.DocumentNode, carVersion);

            return carVersion;
        }

        private void ExtractCarVersion(HtmlNode carVersionNode, CarVersion carVersion)
        {
            string engineAndFuel = ExtractText(carVersionNode, "CarVersion.EngineAndFuel",
                "//div[@class='catalog-item-about-col'][1]/div[@class='catalog-item-about-value']", false);

            if (!string.IsNullOrEmpty(engineAndFuel))
            {
                string[] words = engineAndFuel.Split('/');

                carVersion.Engine = words[0] + words[1];
                carVersion.FuelType = words[2];
            }

            carVersion.Drive = null;

            carVersion.EnvironmentalStandard = null;

            carVersion.Transmission = ExtractText(carVersionNode, "CarVersion.Transmission",
                "//div[@class='catalog-item-about-col'][2]/div[@class='catalog-item-about-value']", false);

            carVersion.FuelConsumptionInCity = GetFuelData(carVersionNode, "CarVersion.FuelConsumptionInCity",
                "//table[@class='table']//tr[3]/td[2]", false);

            carVersion.FuelConsumptionOutCity = GetFuelData(carVersionNode, "CarVersion.FuelConsumptionOutCity",
                "//table[@class='table']//tr[4]/td[2]", false);

            carVersion.FuelConsumptionAverage = GetFuelData(carVersionNode, "CarVersion.FuelConsumptionAverage",
                "//table[@class='table']//tr[5]/td[2]", false);
        }
    }
}