using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebSpider.WebParsers;

namespace WebSpider
{
    public class Parser: IParser
    {
        public static async System.Threading.Tasks.Task<string> GetBook(string webAdress)
        {
            string TEST = "https://novelfull.com/index.php/release-that-witch.html?page=1&per-page=50";
            var MEGATEST = await GetChaptersLinksList(TEST, "//*[@class='chapter-text']");
            string BookText = "";
            foreach (var pagelink in MEGATEST)
            {
                var page = await GetWebPage(pagelink);
                var texts = page.DocumentNode.Descendants("p");
                foreach (var text in texts)
                {
                    BookText += text.InnerText;
                }
            }
            return BookText;
            
            //return await GetTextFromPage(webAdress, "(//div[contains(@class,'desc-text')])[1]");
        }

        private static async Task<string> GetTextFromPage(string webAdress, string xpathExpression)
        {

            var pageDocument = await GetWebPage(webAdress);
            return pageDocument.DocumentNode.SelectSingleNode(xpathExpression).InnerText;
        }

        private static async System.Threading.Tasks.Task<List<string>> GetChaptersLinksList(string webAdress, string xpathExpression)
        {
            var pageDocument = await GetWebPage(webAdress);
            List<string> webLinks = new List<string>();
            do
            {
                var nodes = pageDocument.DocumentNode.SelectNodes(xpathExpression);
                foreach (var node in nodes.Skip(5))
                {
                    var nodeContent = node.ParentNode.ParentNode.LastChild.OuterHtml;
                    nodeContent = "https://novelfull.com" + nodeContent.Remove(0, 9);
                    int index = nodeContent.IndexOf("title=");
                    nodeContent = nodeContent.Remove(index).Replace("\"", string.Empty);
                    webLinks.Add(nodeContent);
                }
                if (pageDocument.DocumentNode.SelectSingleNode("//*[contains(@class, 'next')]") == null)
                    break;
                else
                {
                    var nextPageLink = pageDocument.DocumentNode.SelectSingleNode("//*[contains(@class, 'next')]").InnerHtml;
                    nextPageLink = "https://novelfull.com" + nextPageLink.Remove(0, 9);
                    int index = nextPageLink.IndexOf("data-page");
                    nextPageLink = nextPageLink.Remove(index).Replace("\"", string.Empty);
                    pageDocument = await GetWebPage(nextPageLink);
                }
            } while (pageDocument.DocumentNode.SelectSingleNode("//*[contains(@class, 'last disabled')]") == null);
            return webLinks;
        }

        private static async System.Threading.Tasks.Task<HtmlDocument> GetWebPage(string webAdress)
        {
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(webAdress);
            var pageContents = await response.Content.ReadAsStringAsync();
            HtmlDocument pageDocument = new HtmlDocument();
            pageDocument.LoadHtml(pageContents);
            return pageDocument;
        }


        Task<string> IParser.GetTextAsync(string webAdress)
        {
            throw new NotImplementedException();
        }
    }
}
