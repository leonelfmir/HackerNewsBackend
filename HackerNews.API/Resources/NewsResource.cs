using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerNews.API.Resources
{
    public class NewResource
    {
        public string Author { get; set; }
        public DateTime Time { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
    }
}
