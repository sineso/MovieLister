using MovieList.MovieTextParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MovieList.IgnoreMovies
{
    public class IgnoreMoviesService
    {
        private string filePath;
        private List<Regex> ignoreRegexes;

        public IgnoreMoviesService(string filePath, List<string> ignorePatterns)
        {
            this.filePath = filePath;

            this.ignoreRegexes = new List<Regex>();
            foreach (var ignorePattern in ignorePatterns)
            {
                this.ignoreRegexes.Add(new Regex(ignorePattern, RegexOptions.IgnoreCase));
            }
        }
        public List<Movie> StripIgnoredMovies(List<Movie> movies)
        {
            // Filter out movies saved to the ignore list.
            movies = this.ApplyIgnoreList(movies);

            return movies;
        }

        public List<ParsedMovie> StripIgnoredMovies(List<ParsedMovie> movies)
        {
            // Filter out movies saved to the ignore list.
            movies = this.ApplyIgnoreList(movies);

            // Filter out certain movies, based on regex patterns.
            movies = this.ApplyIgnoreRegex(movies);

            return movies;
        }

        private List<ParsedMovie> ApplyIgnoreRegex(List<ParsedMovie> movies)
        {
            return movies.Where(x =>
            {
                foreach (var regex in ignoreRegexes)
                {
                    if (regex.IsMatch(x.OriginalText))
                    {
                        return false;
                    }
                }

                return true;

            }).ToList();
        }

        public void AddIgnoredMovie(string name)
        {
            if (System.IO.File.Exists(this.filePath))
            {
                System.IO.File.AppendAllLines(this.filePath, new string[] { name });
            }
        }

        private List<Movie> ApplyIgnoreList(List<Movie> movies)
        {
            string[] ignore = new string[0];
            if (System.IO.File.Exists(this.filePath))
            {
                ignore = System.IO.File.ReadAllLines(this.filePath);
            }

            var ignoreMovies = ignore
                .Select(z => z.Trim())
                .Select(z => z.ToLower())
                .ToList();

            return movies
                .Where(x => !ignoreMovies.Any(z => x.Title.ToLower().StartsWith(z)))
                .Where(x => !ignoreMovies.Any(z => (x.Title.ToLower() + " (" + x.Year + ")").StartsWith(z)))
                .ToList();
        }

        private List<ParsedMovie> ApplyIgnoreList(List<ParsedMovie> movies)
        {
            string[] ignore = new string[0];
            if (System.IO.File.Exists(this.filePath))
            {
                ignore = System.IO.File.ReadAllLines(this.filePath);
            }

            var ignoreMovies = ignore
                .Select(z => z.Trim())
                .Select(z => z.ToLower())
                .ToList();

            return movies
                .Where(x => !ignoreMovies.Any(z => x.Title.ToLower().StartsWith(z)))
                .Where(x => !ignoreMovies.Any(z => (x.Title.ToLower() + " (" + x.Year + ")").StartsWith(z)))
                .ToList();
        }
    }
}
