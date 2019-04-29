using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MovieList.MovieTextParser
{
    public static class MovieTextParserUtil
    {   
        /// <summary>
        /// Returns the title component of a torrent movie name.
        /// </summary>
        public static string GetTitle(string torrentMovieName)
        {
            var year = GetYear(torrentMovieName);
            return GetTitle(torrentMovieName, year);
        }

        /// <summary>
        /// Returns the title component of a torrent movie name.
        /// If the year has already been computed, it can be passed in to save having to
        /// computer it again.
        /// </summary>
        public static string GetTitle(string torrentMovieName, string year)
        {
            var index = -1;

            // Attempt 1 - Assume the name is everything up to the year.
            if (!string.IsNullOrEmpty(year))
            {
                index = torrentMovieName.IndexOf(year);
                if (index > -1)
                {
                    torrentMovieName = torrentMovieName.Substring(0, index);
                    return _CleanString(torrentMovieName);
                }
            }

            // Attempt 2 - Use brackets to find the name.
            var regex = MovieTextRegularExpressions.titleUpToBrackets;
            var match = regex.Match(torrentMovieName);
            if (match.Success)
            {
                return _CleanString(match.Groups["title"].Value);
            }

            // Attempt 3 - Assume the name is everything up to the first 2 digit or more number.
            regex = MovieTextRegularExpressions.titleUpToNumber;
            match = regex.Match(torrentMovieName);
            if (match.Success)
            {
                return _CleanString(match.Groups["title"].Value);
            }

            // Attempt 4 = Assume the movie name is the entire string.
            return _CleanString(torrentMovieName);
        }

        /// <summary>
        /// Returns the year component of a movie torrent name.
        /// </summary>
        public static string GetYear(string torrentMovieName)
        {
            // List of regular expressions to use to try and get the year.

            // They are ordered by certainty. For example, a movie year surrounded
            // by brackets is much more likely to be a movie year than one not
            // surrounded by brackets.
            var matchAttempts = new List<Regex>()
            {
                MovieTextRegularExpressions.yearInBrackets,
                MovieTextRegularExpressions.yearInPeriods,
                MovieTextRegularExpressions.yearInWhiteSpace,
                MovieTextRegularExpressions.yearInText
            };

            foreach (var matchAttempt in matchAttempts)
            {
                var match = matchAttempt.Match(torrentMovieName);
                if (match.Success)
                {
                    return match.Groups["year"].Value;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Strip non-alphanumeric characters from the specified string.
        /// Condense whitespace to maximum of one space.
        /// </summary>
        private static string _CleanString(string text)
        {
            text = Regex.Replace(text, @"[^\w\d]", " ");
            text = Regex.Replace(text, @"\s+", " ");
            text = text.ToLower().Trim();

            return text;
        }
    }
}
