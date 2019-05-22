using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CarBase.Business;

namespace CarBase.WebUI.Models
{
    public class CatalogModel : PageModelBase
    {
        public List<Brand> Brands { get; set; }

        public string keyword { get; set; }

        public CatalogModel(List<Brand> brands)
        {
            Brands = brands;
        }
    }
}