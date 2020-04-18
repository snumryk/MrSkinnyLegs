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

        private static async System.Threading.Tasks.Task<string> GetTextFromPage(string webAdress, string xathExpression)
        {
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(webAdress);
            var pageContents = await response.Content.ReadAsStringAsync();
            HtmlDocument pageDocument = new HtmlDocument();
            pageDocument.LoadHtml(pageContents);
            var pageText = pageDocument.DocumentNode.SelectSingleNode(xathExpression).InnerText;
            return pageText;
        }

        Task<string> IParser.GetTextAsync(string webAdress)
        {
            throw new NotImplementedException();
        }
    }
}
