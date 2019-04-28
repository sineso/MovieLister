using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MovieList.MovieTextParser
{
   public static class MovieTextRegularExpressions
    { 
        // The part of the regular expression to match the year.
        // We look for specific numbers to ensure a match.
        // Ie. we know there are no movies in the year '1800'.
        private static string regex_year_part = "(?<year>((19)|(20))[0-9][0-9])";

        // Match a year in brackets. For example: 'Movie Name (2012)'
        public static Regex yearInBrackets = new Regex(@"\(" + regex_year_part + @"\)");

        //  Match a year in periods. For example: 'Movie.Name.2012'
        public static Regex yearInPeriods = new Regex(@"\." + regex_year_part + @"(\.|$)");

        // Match a year in whitespace. For example: 'Movie name 2012'
        public static Regex yearInWhiteSpace = new Regex(@"\s" + regex_year_part + @"(\s|$)");

        // Match a year surrounded in text. For example 'Move.Name2012'
        public static Regex yearInText = new Regex(@"[^0-9]" + regex_year_part + @"([^0-9]|$)");

        // Match the title by including all text up to the first bracket.
        public static Regex titleUpToBrackets = new Regex(@"^(?<title>.+)[\[\(]+");

        // Match the title by including text up to the first two or more digit number.
        public static Regex titleUpToNumber = new Regex(@"^(?![0-9]{2})(?<title>[^0-9]+)");
    }
}
