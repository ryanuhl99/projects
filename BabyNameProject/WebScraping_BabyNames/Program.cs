using System;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Threading.Tasks;
using System.IO;


namespace WebScraping_BabyNames
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var girlUrl = "https://www.parents.com/top-1000-baby-girl-names-2757832";
            var girlNames = await GirlScrape(girlUrl);
            var path = "C:/Users/19498/Downloads/girlNames.csv"; 

            var boyUrl = "https://www.parents.com/top-1000-baby-boy-names-2757618";
            var boyNames = await BoyScrape(boyUrl);
            var path2 = "C:/Users/19498/Downloads/boyNames.csv";

            SaveToCsv(girlNames, path);
            SaveToCsv(boyNames, path2);
        }

        static async Task<List<string>> GirlScrape(string url)
        {
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var girlNodes = htmlDocument.DocumentNode.SelectNodes("//li");
            var girlNodeContainer = new List<string>();

            if (girlNodes != null)
            {
                foreach (var node in girlNodes)
                {
                    girlNodeContainer.Add(node.InnerText.Trim());
                }
            }

            return girlNodeContainer;
        }

        static void SaveToCsv(List<string> items, string filepath)
        {
            var csv = new StringBuilder();
            foreach (var item in items)
            {
                csv.AppendLine(item);
            }
            File.WriteAllText(filepath, csv.ToString());
        }

        static async Task<List<string>> BoyScrape(string url)
        {
            var httpClient2 = new HttpClient();
            var html2 = await httpClient2.GetStringAsync(url);

            var htmlDocument2 = new HtmlDocument();
            htmlDocument2.LoadHtml(html2);

            var boyNodes = htmlDocument2.DocumentNode.SelectNodes("//li");
            var boyNodeContainer = new List<string>();

            if (boyNodes != null)
            {
                foreach (var node in boyNodes)
                {
                    boyNodeContainer.Add(node.InnerText.Trim());
                }
            }

            return boyNodeContainer;
        } 
    }
}