using System.ComponentModel.DataAnnotations;

namespace Blog_App_Dev.Models
{
    public class Movie
    {
        [Key]
        [Required]
        public int ID { get; set; }
        [Required]
        public string Type { get; set; } = "";
        public string Overview { get; set; } = "";
        public ICollection<RegionInfo>? Regions { get; set; }
        public ICollection<Actor>? Cast { get; set; }
        public int ReleaseYear { get; set; } = 0;
        public string imdbId { get; set; } = "";
        public int imdbRating { get; set; } = 0;
        public int imdbVoteCount { get; set; } = 0;
        public int tmdbId { get; set; } = 0;
        public int tmdbRating { get; set; } = 0;
        public string OriginalTitle { get; set; } = "";
        public string BackdropPath { get; set; } = "";
        public ICollection<Genre>? Genres { get; set; }
        public ICollection<Director>? Directors { get; set; }
        public int Runtime { get; set; } = 0;
        public string Trailer { get; set; } = "";
        public string TrailerID { get; set; } = "";
        public string PosterPath { get; set; } = "";
        public string Tagline { get; set; } = "";
    }
}
