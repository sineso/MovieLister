using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieList.MovieTextParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieList.Test.MovieTextParser
{
    [TestClass]
    public class MovieTextParserUtilTest
    {

        [TestMethod]
        public void GetTitleTest()
        {
            var testCases = new List<Tuple<string, string>>();

            testCases.Add(Tuple.Create
            (
                "Glass.2019.1080p.WEBRip.x264-MP4p",
                "Glass"
            ));

            testCases.Add(Tuple.Create
            (
                "How.To.Train.Your.Dragon.The.Hidden.World.2019.1080p.BRRip.x264",
                "How To Train Your Dragon The Hidden World"
            ));

            testCases.Add(Tuple.Create
            (
                "Avengers Endgame 2019 English 720p HDCAM V3 x264 2.7GB[MB]",
                "Avengers Endgame"
            ));

            testCases.Add(Tuple.Create
            (
                "Avengers: Age of Ultron(2015) 1080p BrRip x264 - YIFY",
                "Avengers Age of Ultron"
            ));

            testCases.Add(Tuple.Create
            (
                "Avengers: Age of Ultron 1080p BrRip x264 - YIFY",
                "Avengers Age of Ultron"
            ));

            testCases.Add(Tuple.Create
            (
                "Avengers: Age of Ultron [x264]",
                "Avengers Age of Ultron"
            ));

            testCases.Add(Tuple.Create
            (
                "Avengers - Age of Ultron 2015",
                "Avengers Age of Ultron"
            ));

            testCases.Add(Tuple.Create
            (
                "Avengers Age of Ultron (720p)",
                "Avengers Age of Ultron"
            ));

            foreach (var testCase in testCases)
            {
                var title_parsed = MovieTextParserUtil.GetTitle(testCase.Item1);
                var title_expected = testCase.Item2;

                Assert.AreEqual(title_expected, title_parsed);
            }
        }


        [TestMethod]
        public void GetYearTest()
        {
            var testCases = new List<Tuple<string, string>>();

            testCases.Add(Tuple.Create("Glass.2019.1080p.WEBRip.x264-MP4p", "2019"));
            testCases.Add(Tuple.Create("How.To.Train.Your.Dragon.The.Hidden.World.2019.1080p.BRRip.x264", "2019"));
            testCases.Add(Tuple.Create("Avengers Endgame 2019 English 720p HDCAM V3 x264 2.7GB[MB]", "2019"));
            testCases.Add(Tuple.Create("Creed.2.2018.1080p.WEBRip.x264-MP4", "2018"));
            testCases.Add(Tuple.Create("Avengers Infinity War (2018)", "2018"));
            testCases.Add(Tuple.Create("Avengers Infinity War (2018", "2018"));
            testCases.Add(Tuple.Create("Avengers Infinity War 2018", "2018"));
            testCases.Add(Tuple.Create("Avengers Infinity War 2018 HD", "2018"));

            foreach (var testCase in testCases)
            {
                var year_parsed = MovieTextParserUtil.GetYear(testCase.Item1);
                var year_expected = testCase.Item2;

                Assert.AreEqual(year_expected, year_parsed);
            }
        }
    }
}
