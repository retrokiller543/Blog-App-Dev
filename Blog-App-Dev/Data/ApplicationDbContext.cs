using Blog_App_Dev.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Blog_App_Dev.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Comment> CommentPosts { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<StreamingService> StreamingServices { get; set; }
        public DbSet<RegionInfo> RegionInfo { get; set; }
        public DbSet<Audios> Audios { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Actor> Casts { get; set; }
        public DbSet<Director> Directors { get; set; }
        public DbSet<Subtitles> Subtitles { get; set; }
    }
}