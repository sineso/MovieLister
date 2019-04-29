using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieList.IMDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieList.Test.IMDB
{
    [TestClass]
    public class ImdbServiceTest
    {

        [TestMethod]
        [DeploymentItem(@"ImdbSample.html")]
        public void StripMovieNamesFromHtml()
        {
            var imdbService = new ImdbService(null, new Config.ConfigFile()
            {
                imdb = new Config.ImdbConfig()
                {
                },
            });

            var page = string.Concat(File.ReadAllLines("ImdbSample.html").ToList());
            var movies = imdbService.StripMovieNamesFromHtml(page);

            Assert.AreEqual(page, page);
        }
    }
}
