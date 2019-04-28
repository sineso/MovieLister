using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieList
{
    public class Movie
    {
        public string OriginalTitle { get; set; }
        public string Year { get; set; }
        public double? Rating { get; set; }
        public string Overview { get; set; }
        public string Title { get; internal set; }





        internal string print()
        {
            var text = Title;

            if (!string.IsNullOrEmpty(Year))
            {
                text += " (" + this.Year + ")";
            }

            if (Rating != null)
            {
                text = "[" + Rating.Value.ToString("0.0") + "] " + text;
            }
            else
            {
                text = "[?] " + text;
            }

            System.Console.WriteLine("========================================================");
            System.Console.WriteLine(text);

            if (!string.IsNullOrEmpty(Overview))
            {
                System.Console.WriteLine("\n" + string.Concat(Overview.Take(500)));
            }

            return text;
        }
    }
}
