using CarBase.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace CarBase.CrawlerManager
{
    public abstract class CrawlerBase
    {
        protected CrawlerBase(Logger logManager)
        {
            this.LogManager = logManager;
        }

        protected Logger LogManager { get; }

        protected void LogCar(Model model)
        {
            LogManager.Info("Model Info: " + model.Name + "; " + model.PriceFrom + "; " + model.Link + ";");
            LogManager.Info("Available Versions: \n");
            if (model.Versions != null)
                foreach (var car in model.Versions)
                    LogManager.Info("Car Info: " + car.Drive + "; " + car.Engine + "; " + car.EnvironmentalStandard + "; "
                        + car.FuelType + "; " + car.Price + "; " + car.Transmission + "; " + car.FuelConsumptionInCity + "; "
                        + car.FuelConsumptionOutCity + ";");
            LogManager.Info("<------------------------------------>");
        }
    }
}
