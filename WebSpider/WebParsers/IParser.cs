using System;
using System.Collections.Generic;
using System.Text;

namespace WebSpider.WebParsers
{
    interface IParser
    {
        System.Threading.Tasks.Task<string> GetTextAsync(string webAdress);
    }
}
