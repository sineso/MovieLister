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

            var config_lines= File.ReadAllLines(config_file);

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

            var ignoreFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "IgnoreMovies.txt");

            var ignoreMoviesService = new IgnoreMoviesService(ignoreFile, config.ignore_regexes);
            var webDownloadService = new WebDownloadService();
            var movieTextParserService = new MovieTextParserService();
            var pirateBayService = new PirateBayService(webDownloadService, movieTextParserService, config.piratebay_url, config.piratebay_regex);

            int startPage = 1;
            int pages = 3;

            if (args.Length > 1 && args[0].ToUpper() == "IGNORE")
            {
                var name = string.Join(" ", args.Skip(1)).Trim();
                ignoreMoviesService.AddIgnoredMovie(name);
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

            // Get the most seeded movies from the Pirate Bay listing.
            var parsedMovies = pirateBayService.GetMostSeededMovies(startPage, pages);

            // Strip out all movies the user has chosen to ignore.
            parsedMovies = ignoreMoviesService.StripIgnoredMovies(parsedMovies);

            // Limit the movie list to 15.
            parsedMovies = parsedMovies.Take(15).ToList();

            // Use TheMovieDb API to get information about these movies.
            var theMovieDbService = new TheMovieDbService(webDownloadService, config.themoviedb_url, config.themoviedb_key);
            var movies = Program.GetMovies(theMovieDbService, parsedMovies);

            // In case the downloaded title is different than the torrent title,
            // run strip again but this time against downloaded titles.
            movies = ignoreMoviesService.StripIgnoredMovies(movies);

            // Sort by rating and take the top 5.
            movies = movies
                .OrderByDescending(x => x.Rating)
                .Take(5)
                .ToList();

            // Print the results.
            foreach (var movie in movies)
            {
                movie.print();
                Console.WriteLine();
            }
        }     

        private static List<Movie> GetMovies(TheMovieDbService theMovieDbService, List<ParsedMovie> parsedMovies)
        {
            Console.Write("Downloading ");

            var movies = new List<Movie>();
            foreach (var parsedMovie in parsedMovies)
            {
                var responseMovie = theMovieDbService.GetMovie(parsedMovie.Title, parsedMovie.Year);
                Console.Write(".");

                if (responseMovie == null)
                {
                    continue;
                }

                movies.Add(new Movie()
                {
                    OriginalTitle = parsedMovie.Title,
                    Title = responseMovie.title,
                    Rating = responseMovie.vote_average,
                    Year = string.Concat(responseMovie.release_date.Take(4)),
                    Overview = responseMovie.overview
                });
            }

            Console.WriteLine();

            return movies;
        }
    }
}