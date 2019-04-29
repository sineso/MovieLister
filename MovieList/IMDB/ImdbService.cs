using System;
using System.Collections.Generic;
using MovieList.Config;
using MovieList.MovieTextParser;
using MovieList.WebDownload;
using System.Text.RegularExpressions;

namespace MovieList.IMDB
{
    public class ImdbService
    {
        private WebDownloadService webDownloadService;

        private string url;
        private Regex whiteSpaceRemove = new Regex(@"\s+");

        public ImdbService(WebDownloadService webDownloadService, ConfigFile config)
        {
            this.webDownloadService = webDownloadService;
            this.url = config.imdb.url;
        }

        public List<ParsedMovie> GetTitles()
        {
            // Download the HTML.
            var html = this.webDownloadService.DownloadPage(url);

            // Strip movie titles from the html.
            var movies = this.StripMovieNamesFromHtml(html);

            return movies;
        }

        public List<ParsedMovie> StripMovieNamesFromHtml(string html)
        {
            html = whiteSpaceRemove.Replace(html, " ");

            var movies = new List<ParsedMovie>();

            var h3 = "<h3 class=\"lister-item-header\">";
            var h3_end = "</h3>";

            var regex = new Regex(">(?<title>[^>]+)<\\/a>.+>\\((?<year>[0-9]{4})\\)<");

            while (true)
            {
                var index = html.IndexOf(h3);
                if (index < 0)
                {
                    break;
                }

                html = html.Substring(index);
                index = html.IndexOf(h3_end);

                var movie_part = html.Substring(0, index);

                var match = regex.Match(movie_part);

                if (match.Success)
                {
                    var title = match.Groups["title"].Value;
                    title = MovieTextParserUtil.CleanString(title);

                    var year = match.Groups["year"].Value.ToLower().Trim();

                    movies.Add(new ParsedMovie()
                    {
                        OriginalText = string.Empty,
                        Title = title,
                        Year = year,
                    });
                }

                html = html.Substring(index);
            }

            return movies;
        }
    }
}