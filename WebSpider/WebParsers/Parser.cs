using HtmlAgilityPack;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using WebSpider.WebParsers;

namespace WebSpider
{
    public class Parser: IParser
    {
        public static async System.Threading.Tasks.Task<string> GetBook(string webAdress)
        {
            return await GetTextFromPage(webAdress, "(//div[contains(@class,'desc-text')])[1]");
        }

        private static async Task<string> GetTextFromPage(string webAdress, string xathExpression)
        {

            var pageDocument = await GetWebPage(webAdress);
            return pageDocument.DocumentNode.SelectSingleNode(xathExpression).InnerText;
        }

        private static async System.Threading.Tasks.Task<string> GetPagesLinksList()
        {
            return null;
        }

        private static async System.Threading.Tasks.Task<string> GetChaptersLinksList()
        {
            return null;
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
