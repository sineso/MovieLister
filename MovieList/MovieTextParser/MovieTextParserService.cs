using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MovieList.MovieTextParser
{
    public class MovieTextParserService
    {
        public ParsedMovie Execute(string text)
        {
            var year = MovieTextParserUtil.GetYear(text);
            var title = MovieTextParserUtil.GetTitle(text, year);

            if (string.IsNullOrEmpty(title))
            {
                return null;
            }

            return new ParsedMovie()
            {
                OriginalText = text,
                Year = year,
                Title = title
            };
        } 
    }
}
