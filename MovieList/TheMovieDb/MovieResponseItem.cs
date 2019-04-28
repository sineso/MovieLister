namespace MovieList.TheMovieDb
{
    public class MovieResponseItem
    {
        public int vote_count { get; set; }
        public int id { get; set; }
        public bool video { get; set; }
        public double vote_average { get; set; }
        public string title { get; set; }
        public double popularity { get; set; }
        public string overview { get; set; }
        public string release_date { get; set; }
    }
}