using CarBase.Business;
using System.Collections.Generic;

namespace CarBase.CrawlerManager
{
    public interface ICrawler
    {
        List<Model> GetCars();
    }
}
