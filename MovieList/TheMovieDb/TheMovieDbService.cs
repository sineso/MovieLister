using MovieList.MovieTextParser;
using MovieList.TheMovieDb;
using MovieList.WebDownload;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MovieList.TheMovieDb
{
    public class TheMovieDbService
    {
        private WebDownloadService webDownloadService;
        private string api_url;
        private string api_key;

        public TheMovieDbService(WebDownloadService webDownloadService, string api_url, string api_key)
        {
            this.webDownloadService = webDownloadService;

            this.api_key = api_key;
            this.api_url = api_url;
        }

        private string FormatUrl(string title, string year)
        {
            var title_url = HttpUtility.UrlEncode(title);
            var year_url = HttpUtility.UrlEncode(year);

            return string.Format(api_url, api_key, title_url, year_url);
        }

        public MovieResponseItem GetMovie(string title, string year)
        {
            if (string.IsNullOrEmpty(title))
            {
                return null;
            }

            if (string.IsNullOrEmpty(year))
            {
                return null;
            }

            // Format the URL, which is a Get request to TheMovieDb.
            var url = this.FormatUrl(title, year);

            // Peform the call using webdownload service.
            var response_json = webDownloadService.DownloadPage(url);

            // Deserialize the JSON.
            var response_obj = JsonConvert.DeserializeObject<MovieResponse>(response_json);
            if (response_obj.results.Count == 0)
            {
                return null;
            }

            return response_obj.results[0];
        }
    }
}
