using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerNews.API.Resources
{
    public class NewsSearchResource
    {
        [Required]
        public string Search { get; set; }
    }
}
