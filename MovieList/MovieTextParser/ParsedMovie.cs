using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieList.MovieTextParser
{
    public class ParsedMovie
    {
        public string OriginalText { get; set; }
        public string Title { get; set; }
        public string Year { get; set; }

        /// <summary>
        /// A simple string representation of this movie.
        /// </summary>
        public string SimpleString()
        {
            var text = Title;

            var textInfo = new CultureInfo("en-US", false).TextInfo;
            text = textInfo.ToTitleCase(text);

            if (!string.IsNullOrEmpty(Year))
            {
                text += " (" + Year + ")";
            }

            return text;
        }
    }
}
