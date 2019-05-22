using HtmlAgilityPack;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CarBase.Business;

namespace CarBase.CrawlerManager
{
    public abstract class HtmlAgilityCrawlerBase : CrawlerBase, ICrawler
    {
        protected HtmlAgilityCrawlerBase(Logger logManager) : base(logManager)
        {
        }

        protected HtmlDocument LoadHtmlDocument(string url)
        {
            int attempt = 60000;
            HtmlDocument document = null;
            HtmlWeb web = new HtmlWeb();
            while (attempt < 3840000)
            {
                try
                {
                    document = web.Load(Domain + url);
                    break;
                }
                catch (HtmlWebException ex)
                {
                    LogManager.Error(ex, $"\nPage load error. Url={url}. Error: {ex.Message}");
                    Thread.Sleep(attempt);
                    attempt *= 4;
                }
            }
            return document;
        }
        
        protected HtmlNode GetFieldNode(HtmlNode node, string id, string xPath, bool isRequired)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));
            if (id == null)
                throw new ArgumentNullException(nameof(id));
            if (xPath == null)
                throw new ArgumentNullException(nameof(xPath));

            HtmlNodeCollection nodes = node.SelectNodes(xPath);

            // required fields must be extracted
            if ((nodes == null) || (nodes.Count == 0))
            {
                if (isRequired)
                    throw new ExtractionException($"Required field {id} wasn't found in the page html.");

                return null;
            }

            if (nodes.Count > 1)
            {
                throw new ExtractionException($"XPath returned more than 1 node for field {id}.");
            }

            return nodes[0];
        }

        protected string ExtractLink(HtmlNode node, string id, string xPath, bool isRequired)
        {
            HtmlNode fieldNode = GetFieldNode(node, id, xPath, isRequired);

            if (fieldNode == null)
                return null;

            HtmlAttribute attr = fieldNode.Attributes["href"];
            if (attr == null)
                throw new ExtractionException($"Field {id} of Link type doesn't have href attribute");

            string result = attr.Value;

            if (isRequired && string.IsNullOrEmpty(result))
                throw new ExtractionException($"Field {id} cannot have empty link.");

            return result;
        }

        protected int ExtractPrice(HtmlNode node, string id, string xPath, bool isRequired)
        {
            string priceString = ExtractText(node, id, xPath, isRequired);
            string answer = "";
            Regex rgx = new Regex(@"\d", RegexOptions.Compiled);
            foreach (Match match in rgx.Matches(priceString))
                answer += match.Value;
            return Convert.ToInt32(answer);
        }

        protected string ExtractText(HtmlNode node, string id, string xPath, bool isRequired)
        {
            var resultNode = GetFieldNode(node, id, xPath, isRequired);

            if (resultNode != null)
            {
                if (isRequired && string.IsNullOrEmpty(resultNode.InnerText))
                    throw new ExtractionException($"Field {id} cannot have empty value.");
                return resultNode.InnerText;
            }

            return "";
        }

        protected HtmlNodeCollection ExtractNodeCollection(HtmlNode node, string xPath)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));
            if (xPath == null)
                throw new ArgumentNullException(nameof(xPath));

            HtmlNodeCollection htmlNodes = node.SelectNodes(xPath);

            if (htmlNodes == null)
                return null;

            return htmlNodes;
        }

        protected string RemoveChildNodes(HtmlNode oldNode, string xpath)
        {
            var list = oldNode.SelectSingleNode(xpath);
            if (list.SelectSingleNode("./span") == null)
                return xpath;
            list.RemoveChild(list.SelectSingleNode("./span"));
            return list.XPath;
        }

        protected double? GetFuelData(HtmlNode carVersionNode, string id, string xpath, bool isRequired)
        {
            string fuelString = ExtractText(carVersionNode, id, xpath, isRequired);
            if (fuelString != "")
            {
                string[] mem = fuelString.Split(' ');
                return Convert.ToDouble(mem[0].Replace('.', ','));
            }

            return null;
        }

        protected abstract List<Model> GetCars(HtmlDocument document);

        public List<Model> GetCars()
        {
          HtmlDocument document = LoadHtmlDocument(CatalogUrl);
          return GetCars(document);
        }

        protected abstract string Domain { get; }

        protected abstract string CatalogUrl { get; }
    }
}
