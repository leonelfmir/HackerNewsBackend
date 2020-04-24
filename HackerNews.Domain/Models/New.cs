﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerNews.Domain.Models
{
    public class New
    {
        public int Id { get; set; }
        public string By { get; set; }
        public DateTime Time { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
    }
}
