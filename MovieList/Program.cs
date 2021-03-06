﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MovieList.TheMovieDb;
using MovieList.PirateBay;
using MovieList.MovieTextParser;
using MovieList.WebDownload;
using MovieList.IgnoreMovies;
using Newtonsoft.Json;
using MovieList.Config;
using System.Text;
using MovieList.IMDB;
using System.Threading.Tasks;

namespace MovieList
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config_file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
            if (!File.Exists(config_file))
            {
                Console.WriteLine("Error: config.json file missing.");
                return;
            }

            var config_lines = File.ReadAllLines(config_file);

            // Remove comments.
            var config_contents = config_lines
                .Where(x => !x.Trim().StartsWith(@"\\"))
                .Aggregate((x, y) => x + "\n" + y);

            var config = JsonConvert.DeserializeObject<ConfigFile>(config_contents);
            if (config == null)
            {
                Console.WriteLine("Error: Unable to parse config.json file.");
                return;
            }

            if (string.IsNullOrEmpty(config.themoviedb_key))
            {
                // Base 64 encoded so API key cannot easily be stripped by bots reading this source code.
                var bytes = Convert.FromBase64String("M2QwYjY4Y2Q1ZGJlZjYyNzFjOGVhZWYxNzU1YjBmNGU=");
                config.themoviedb_key = Encoding.Default.GetString(bytes);
            }

            var webDownloadService = new WebDownloadService();
            var movieTextParserService = new MovieTextParserService();
            var ignoreMoviesService = new IgnoreMoviesService(movieTextParserService, config.ignore_regexes);
            var pirateBayService = new PirateBayService(webDownloadService, movieTextParserService, config);
            var imdbService = new ImdbService(webDownloadService, config);

            int startPage = 1;
            int pages = 3;

            if (args.Length > 1 && args[0].ToUpper() == "IGNORE")
            {
                var name = string.Join(" ", args.Skip(1)).Trim();
                ignoreMoviesService.AddIgnoredMovie(name);
                return;
            }

            if (args.Length > 0 && args[0].ToUpper() == "IGNORED")
            {
                ignoreMoviesService.PrintIgnored();
                return;
            }

            if (args.Length > 1 && args[0].ToUpper() == "PAGE")
            {
                startPage = int.Parse(args[1]);
                if (startPage < 0)
                {
                    startPage = 1;
                }
            }

            var parsedMovies = new List<ParsedMovie>();

            // Get the most seeded movies from the Pirate Bay listing.
            if (config.imdb.enable)
            {
                Console.Write("\n  Scraping PirateBay for top seeded torrents... ");
                var pirateMovies = pirateBayService.GetMostSeededMovies(startPage, pages);
                Console.Write(pirateMovies.Count() + " found");
                parsedMovies.AddRange(pirateMovies);
            }

            // Get recent DVD released from IMDB.
            if (config.imdb.enable)
            {
                Console.Write("\n  Scraping IMDB for new DVD Releases... ");

                try
                {
                    var imdbMovies = imdbService.GetTitles();
                    Console.Write(imdbMovies.Count() + " found");
                    parsedMovies.AddRange(imdbMovies);

                }
                catch (Exception ex)
                {
                    Console.Write("failed");
                    Console.WriteLine(ex.ToString());
                }
            }

            // Ensure the list is distinct.
            parsedMovies = parsedMovies
                .GroupBy(x => new { x.Title, x.Year })
                .Select(x => x.First())
                .ToList();

            // Strip out all movies the user has chosen to ignore.
            parsedMovies = ignoreMoviesService.StripIgnoredMovies(parsedMovies);
            Console.WriteLine("\n  Removing ignored movies... " + parsedMovies.Count() + " remaining");

            // Limit the movie list to 20.
            parsedMovies = parsedMovies.Take(20).ToList();

            // Use TheMovieDb API to get information about these movies.
            var theMovieDbService = new TheMovieDbService(webDownloadService, config.themoviedb_url, config.themoviedb_key);
            var movies = Program.GetMovies(theMovieDbService, parsedMovies);

            // In case the downloaded title is different than the torrent title,
            // run strip again but this time against downloaded titles.
            movies = ignoreMoviesService.StripIgnoredMovies(movies);

            // Sort by rating and take the top 5.
            movies = movies
                .GroupBy(x => new { x.Title, x.Year })
                .Select(x => x.First())
                .OrderByDescending(x => x.Rating)
                .Take(5)
                .ToList();

            // Print the results.
            foreach (var movie in movies)
            {
                Console.WriteLine();
                movie.Print();
            }

            Console.WriteLine();
        }

        private static List<Movie> GetMovies(TheMovieDbService theMovieDbService, List<ParsedMovie> parsedMovies)
        {
            Console.Write("  Fetching metadata...");

            var movies = new List<Movie>();

            var lockObj = new object();
            var options = new ParallelOptions { MaxDegreeOfParallelism = 4 };

            // Download movie info in parallel.
            Parallel.ForEach<ParsedMovie>(parsedMovies, options, parsedMovie =>
           {
               var responseMovie = theMovieDbService.GetMovie(parsedMovie.Title, parsedMovie.Year);

               if (responseMovie == null)
               {
                   return;
               }

               lock (lockObj)
               {
                   movies.Add(new Movie()
                   {
                       OriginalTitle = parsedMovie.Title,
                       Title = responseMovie.title,
                       Rating = responseMovie.vote_average,
                       Year = string.Concat(responseMovie.release_date.Take(4)),
                       Overview = responseMovie.overview
                   });
               }
           });

            Console.WriteLine();

            return movies;
        }
    }
}
