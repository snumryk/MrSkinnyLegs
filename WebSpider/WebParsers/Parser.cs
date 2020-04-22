using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebSpider.BookMakers;
using WebSpider.WebParsers;

namespace WebSpider
{
    public class Parser: IParser
    {
        public static async System.Threading.Tasks.Task GetBook(string webAdress, string filePath)
        {
            var tempDirectory = Directory.CreateDirectory(Path.Combine(filePath, "Chapters"));
            //string TEST = "https://novelfull.com/index.php/release-that-witch.html?page=1&per-page=50";
            var chapterLinks = await GetChaptersLinksList(webAdress, "//*[@class='chapter-text']");
            foreach (var pagelink in chapterLinks)
            {
                var page = await GetWebPage(pagelink);
                var nodes = page.DocumentNode.Descendants("p");
                var ChapterTexts = new List<string>();
                var index = pagelink.IndexOf("release-that-witch/");
                var chapterName = pagelink.Substring(index+19).Replace(".html", ".").TrimEnd() + "txt"; 
                foreach (var text in nodes)
                {
                    string fixedText = text.InnerText.Replace("&mdash", string.Empty)
                                                    .Replace("&#8220;", string.Empty)
                                                    .Replace("&#8221;", string.Empty)
                                                    .Replace("&#8217;", string.Empty);
                    ChapterTexts.Add(fixedText);
                }
                File.WriteAllLines(Path.Combine(tempDirectory.FullName, chapterName), ChapterTexts);
            }
            EPubMaker.MakeBook(filePath);
            //tempDirectory.Delete(true);
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
