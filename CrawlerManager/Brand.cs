using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarBase.CrawlerManager
{
    public class Brand
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string CrawlerType { get; set; }

        public bool IsActive { get; set; }

        public virtual List<Model> Models { get; set; }
    }
}
