using MovieList.MovieTextParser;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MovieList.IgnoreMovies
{
    public class IgnoreMoviesService
    {
        private string filePath;
        private List<ParsedMovie> _ignoredMovies;
        private List<Regex> ignoreRegexes;
        private MovieTextParserService movieTextParserService;

        public IgnoreMoviesService(MovieTextParserService movieTextParserService, List<string> ignorePatterns)
        {
            // Create the ignore movies file, if necessary.
            this.SetupFile();

            this.movieTextParserService = movieTextParserService;

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
            var parsed = this.movieTextParserService.Execute(name);
            var simpleString = parsed.SimpleString();

            if (File.Exists(this.filePath))
            {
                File.AppendAllLines(this.filePath, new string[] { simpleString });
            }
        }

        public List<ParsedMovie> GetIgnoredMovies()
        {
            if (this._ignoredMovies != null)
            {
                return this._ignoredMovies;
            }

            // Read the lines from the file.
            var ignore = new string[0];
            if (File.Exists(this.filePath))
            {
                ignore = File.ReadAllLines(this.filePath);
            }

            // Convert the array into a list.
            var ignore_list = ignore
                .Select(z => z.Trim())
                .Select(z => z.ToLower())
                .ToList();

            // Use the parser service to split items into movie names and years.
            // This also removes non alpha-numeric characters to make string comparisons easier.
            this._ignoredMovies = new List<ParsedMovie>();
            foreach (var ignore_item in ignore_list)
            {
                var movie = movieTextParserService.Execute(ignore_item);
                if (movie != null)
                {
                    this._ignoredMovies.Add(movie);
                }
            }

            return this._ignoredMovies;
        }
        private void SetupFile()
        {
            // Create the directory, unless it already exists.
            var environmentFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            Directory.CreateDirectory(Path.Combine(environmentFolder, "MovieLister"));

            // Create ignore file, unless it already exists.
            var ignoreFile = Path.Combine(environmentFolder, "MovieLister\\IgnoreMovies.txt");
            using (var fileStream = new FileStream(ignoreFile, FileMode.OpenOrCreate))
            {
            }

            this.filePath = ignoreFile;
        }

        private bool IsMatch(List<ParsedMovie> ignoredMovies, string title, string year)
        {
            title = title.ToLower();

            foreach (var ignore in ignoredMovies)
            {
                if (!string.IsNullOrEmpty(ignore.Year))
                {
                    if (ignore.Year != year)
                    {
                        continue;
                    }
                }

                if (ignore.Title.StartsWith(title))
                {
                    return true;
                }
            }

            return false;
        }

        private List<Movie> ApplyIgnoreList(List<Movie> movies)
        {
            var ignoreMovies = this.GetIgnoredMovies();

            return movies
                .Where(x => !this.IsMatch(ignoreMovies, x.OriginalTitle, x.Year))
                .ToList();
        }

        private List<ParsedMovie> ApplyIgnoreList(List<ParsedMovie> movies)
        {
            var ignoreMovies = this.GetIgnoredMovies();

            return movies
                .Where(x => !this.IsMatch(ignoreMovies, x.Title, x.Year))
                .ToList();
        }
    }
}
