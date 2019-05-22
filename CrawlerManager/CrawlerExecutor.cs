using CarBase.Crawlers;
using CarBase.Data;
using NLog;

namespace CarBase.CrawlerManager
{
    class CrawlerExecutor
    {
        private Logger logManager;
        private CarModelDao daoCarModel;

        public CrawlerExecutor(Logger logManager, CarModelDao daoCarModel)
        {
            this.logManager = logManager;
            this.daoCarModel = daoCarModel;
        }

        public void Execute(ICrawler crawler)
        {
            // 1. Crawl all cars
            var cars = crawler.GetCars();

            // 2. Insert cars into database
            foreach (var car in cars)
            {
                daoCarModel.Insert(car);
            }
        }
    }
}
