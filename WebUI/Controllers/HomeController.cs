using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CarBase.WebUI;
using CarBase.WebUI.Models;

namespace WebUI.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Catalog()
        {
            var dao = new CarModelDao();

            var brands = dao.GetAllBrands();

            CatalogModel model = new CatalogModel(brands);

            return View(model);
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Brand(string brandName)
        {
            var dao = new CarModelDao();
            Brand brand = dao.GetBrandByName(brandName);
            if (brand == null)
                return View("NotFound"); 

            BrandInfoModel model = new BrandInfoModel(brand);
            return View(model);
        }

        public ActionResult ModelInfo(string model, string brandName)
        {
            var dao = new CarModelDao();
            Model modelDao = dao.GetModelByName(model);
            if (modelDao == null)
                return View("NotFound"); 

            CarModelViewModel modelView = new CarModelViewModel(modelDao, brandName);
            
            return View(modelView);
        }

        public ActionResult NotFound()
        {
            return View();
        }
    }
}