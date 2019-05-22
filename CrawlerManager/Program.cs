using CarBase.Business;
using CarBase.Crawlers;
using CarBase.Data;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarBase.CrawlerManager
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger logManager = LogManager.GetLogger("default");
            try
            {
                var daoCarModel = new CarModelDao();

                // delete all models
                daoCarModel.DeleteAll();

                // load active crawlers
                var brands = daoCarModel.GetAllBrands().Where(i => i.IsActive);

                foreach (var brand in brands)
                {
                   Type crawlerType = Type.GetType(brand.CrawlerType);
                   ICrawler crawler = (ICrawler)Activator.CreateInstance(crawlerType, logManager);
                   var crawlerExecutor = new CrawlerExecutor(logManager, daoCarModel);
                   crawlerExecutor.Execute(crawler);
                }
            }
            catch (Exception ex)
            {
                logManager.Fatal(ex, "\nKapec !\nException: " + ex.Message +
                                    "\nMethod: " + ex.TargetSite +
                                    "\nStack trace: " + ex.StackTrace);
            }
            Console.WriteLine("All done!");
            Console.ReadKey();
        }
    }
}
