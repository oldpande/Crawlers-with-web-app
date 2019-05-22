using CarBase.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CarBase.WebUI.Helpers;

namespace CarBase.WebUI.Models
{
    public class CarModelViewModel : PageModelBase
    {
        public Model CarModel { get; set; }

        public string Brand { get; set; }

        public List<CarVersion> CarVersions
        {
            get { return CarModel.Versions; }
        }

        public CarModelViewModel(Model model, string brand)
        {
            Brand = brand;
            CarModel = model;

            AddMenuItem(brand, UrlHelper.BrandUrl(brand));
            AddMenuItem(CarModel.Name, UrlHelper.ModelUrl(brand, CarModel.Name));

            foreach (var m in CarModel.Versions)
            {
                if (m.Drive == "" || m.Drive == null)
                    m.Drive = "Нет информации";

                if (m.Engine == "" || m.Engine == null)
                    m.Engine = "Нет информации";

                if (m.EnvironmentalStandard == "" || m.EnvironmentalStandard == null)
                    m.EnvironmentalStandard = "Нет информации";
                else
                {
                    if (m.EnvironmentalStandard == "Евро 5" || m.EnvironmentalStandard == "Евро 6")
                        m.EnvironmentalStandard = "не более 0,5 г/км";
                }

                if (m.FuelType == "" || m.FuelType == null)
                    m.FuelType = "Нет информации";

                if (m.Transmission == "" || m.Transmission == null)
                    m.Transmission = "Нет информации";

                if (m.FuelConsumptionAverage == 0 || m.FuelConsumptionAverage == null)
                    m.FuelConsumptionAverage = (m.FuelConsumptionInCity + m.FuelConsumptionOutCity) / 2;
            }
        }
    }
}