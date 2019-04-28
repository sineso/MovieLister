﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieList.Config
{
    public class ConfigFile
    {
        public string piratebay_url { get; set; }
        public string piratebay_regex { get; set; }
        public string themoviedb_url { get; set; }
        public string themoviedb_key { get; set; }
        public List<string> ignore_regexes { get; set; }
    }
}
