using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CarBase.Business;

namespace CarBase.WebUI.Helpers
{
    public static class AppUrlHelper
    {
        public static string CatalogUrl(this UrlHelper urlHelper)
        {
            return urlHelper.Content($"~/Catalog/");
        }

        public static string BrandLogoUrl(this UrlHelper urlHelper, Brand brand)
        {
            return urlHelper.Content($"~/Content/Image/Brand/{brand.Name}.jpg");
        }

        public static string BrandUrl(this UrlHelper urlHelper, string brand)
        {
            return urlHelper.Content($"~/Catalog/{brand}");
        }

        public static string ModelUrl(this UrlHelper urlHelper, string brand, string model)
        {
            return urlHelper.Content($"~/Catalog/{brand}/{model}");
        }
        public static string AboutUrl(this UrlHelper urlHelper)
        {
            return urlHelper.Content($"~/About");
        }
    }
}