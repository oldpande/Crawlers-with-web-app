using CarBase.Business;
using System.Collections.Generic;

namespace CarBase.Crawlers
{
    public interface ICrawler
    {
        List<Model> GetCars();
    }
}
