using MovieList.MovieTextParser;
using MovieList.WebDownload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MovieList.PirateBay
{
    public class PirateBayService
    {
        private WebDownloadService webDownloadService;
        private MovieTextParserService movieTextParserService;

        private string piratebayUrl;
        private Regex movieTitle;

        public PirateBayService(WebDownloadService webDownloadService, MovieTextParserService movieTextParserService, string piratebay_url, string piratebay_regex)
        {
            this.webDownloadService = webDownloadService;
            this.movieTextParserService = movieTextParserService;
            this.piratebayUrl = piratebay_url;
            this.movieTitle = new Regex(piratebay_regex);
        }

        public List<ParsedMovie> GetMostSeededMovies(int startPage = 1, int pages = 3)
        {
            var movies = new List<ParsedMovie>();

            for (int x = 0; x < pages; x++)
            {
                // Sleep some amount of time so we aren't hammering the server.
                this.Sleep(x);

                // Calculate the current page we are on.
                var pageAt = startPage + x;

                // Download the page and parse the titles.
                movies.AddRange(this.StripPage(pageAt));
            }

            // Ensure the list is distinct.
            movies = movies
                .GroupBy(x => new { x.Title, x.Year })
                .Select(x => x.First())
                .ToList();

            return movies;
        }

        private List<string> StripMovieNamesFromHtml(string html)
        {
            var names = new List<string>();

            foreach (Match match in movieTitle.Matches(html))
            {
                names.Add(match.Groups["title"].Value);
            }

            return names;
        }

        private List<ParsedMovie> StripPage(int page)
        {
            // Format the pirateBay url.
            var url = string.Format(piratebayUrl, page);

            // Download the HTML.
            var html = this.webDownloadService.DownloadPage(url);

            // Strip movie titles from the html.
            var movieTitles = this.StripMovieNamesFromHtml(html);

            // Parse the the titles.
            var movies = new List<ParsedMovie>();
            foreach (var title in movieTitles)
            {
                var parsedMovie = movieTextParserService.Execute(title);
                if (parsedMovie != null)
                {
                    movies.Add(parsedMovie);
                }
            }

            return movies;
        }

        private void Sleep(int loopCount)
        {
            if (loopCount == 0)
            {
                // Don't sleep on first request.
                return;
            }

            if (loopCount % 3 == 0)
            {
                // PirateBay limits to approximently 3 pages per second.
                // Do a big sleep on 4, 7, 10, etc.
                Thread.Sleep(1100);
                return;
            }

            // Sleep a bit for each request after the first.
            Thread.Sleep(150);
        }
    }
}
