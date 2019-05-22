using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CarBase.WebUI.Models
{
    public class PageModelBase
    {
        public List<MenuItem> Menu { get; private set; } = new List<MenuItem>();

        protected UrlHelper UrlHelper
        {
            get
            {
                return new UrlHelper(HttpContext.Current.Request.RequestContext);
            }
        }

        protected void AddMenuItem(string name, string url)
        {
            Menu.Add(new MenuItem { Name = name, Url = url });
        }

        public PageModelBase()
        {
            AddMenuItem("Главная", UrlHelper.Content("~/"));
        }
    }
}