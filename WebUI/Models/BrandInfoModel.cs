using CarBase.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CarBase.WebUI.Helpers;

namespace CarBase.WebUI.Models
{
    public class BrandInfoModel : PageModelBase
    {
        public Brand Brand { get; set; }

        public List<Model> Models
        {
            get { return Brand.Models; }

        }

        public BrandInfoModel(Brand brand)
        {
            Brand = brand;

            AddMenuItem(Brand.Name, UrlHelper.BrandUrl(Brand.Name));
        }
    }
}