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
                logManager.Info("Clearing database...");
                daoCarModel.DeleteAll();

                // load active crawlers
                logManager.Info("Search active bots...");
                var brands = daoCarModel.GetAllBrands().Where(i => i.IsActive);

                foreach (var brand in brands)
                {
                    try
                    {
                        logManager.Info(brand.Name + " bot is starting");
                        Type crawlerType = Type.GetType(brand.CrawlerType);
                        ICrawler crawler = (ICrawler)Activator.CreateInstance(crawlerType, logManager);
                        var crawlerExecutor = new CrawlerExecutor(logManager, daoCarModel);
                        crawlerExecutor.Execute(crawler);
                    }
                    catch (Exception ex)
                    {
                        logManager.Fatal(ex, "\nKapec !\nException: " + ex.Message +
                                    "\nMethod: " + ex.TargetSite +
                                    "\nStack trace: " + ex.StackTrace);
                    }
                }
            }
            catch (Exception ex)
            {
                logManager.Fatal(ex, "\nKapec !\nException: " + ex.Message +
                                    "\nMethod: " + ex.TargetSite +
                                    "\nStack trace: " + ex.StackTrace);
            }
            Console.WriteLine("All done! You can clouse this window.");
            Console.ReadKey();
        }
    }
}
