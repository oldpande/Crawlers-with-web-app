using System;
using System.Collections.Generic;

namespace CarBase.CrawlerManager
{
    public partial class CarVersion
    {
        public int Id { get; set; }

        public int ModelId { get; set; }

        public string Engine { get; set; }

        public string Drive { get; set; }

        public string Transmission { get; set; }

        public int? Price { get; set; }

        public string FuelType { get; set; }

        public string EnvironmentalStandard { get; set; }

        public double? FuelConsumptionInCity { get; set; }

        public double? FuelConsumptionOutCity { get; set; }

        public double? FuelConsumptionAverage { get; set; }

        public virtual Model CarModel { get; set; }
    }
}
