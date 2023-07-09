using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Blog_App_Dev.Data;
using Blog_App_Dev.Models;
using Newtonsoft.Json.Linq;
using System.Net;
using RestSharp;
using Microsoft.CodeAnalysis;
using NuGet.ProjectModel;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using RapidAPI.StreamingAvailability.Client;
using RapidAPI.StreamingAvailability.Client.Models;

namespace Blog_App_Dev.Controllers
{
  [Authorize(Roles = "User,Admin")]
  public class MoviesController : Controller
  {
    private readonly ApplicationDbContext _context;

    private readonly IConfiguration _configuration;

    private readonly Client client;

    public MoviesController(ApplicationDbContext context, IConfiguration configuration)
    {
      _context = context;
      _configuration = configuration;
      string apiKey = _configuration["RapidAPIKey"];
      client = new Client(apiKey);
    }


    public IActionResult SearchMovie(string title, string type)
    {

      // PROBLEM: if users search for movie and we save it we will save the movie
      // but if we then search for the same title but a series or all we will not know 
      // if we have that data saved already or not. i should make a system to check that and also check last search date
      // to see if we need to update the DB
      var moviesInDb = _context.Movies
          .Where(predicate: m => m.Title.Contains(title))
              .Include(m => m.Regions)
              .ThenInclude(r => r.StreamingServices)
              .Include(m => m.Cast)
              .Include(m => m.Directors)
              .Include(m => m.Genres)
          .ToList();

      if (moviesInDb.Any())
      {
        // If any movies are found in the database, return them
        ViewBag.Query = $"{title}";
        return View(moviesInDb);
      } else
      {
        var responsMovies = client.SearchByTitle(title, "se", type, "en");
        if (responsMovies.Any())
        {
          foreach (var movie in responsMovies)
          {
            SaveMovie(movie);
          }
          var movies = _context.Movies
            .Where(m => m.Title.Contains(title))
            .Include(m => m.Regions)
            .ThenInclude(r => r.StreamingServices)
            .Include(m => m.Cast)
            .Include(m => m.Directors)
            .Include(m => m.Genres)
            .ToList();

          ViewBag.Query = $"{title}";

          return View(movies);
        } else
        {
          return NotFound();
        }
      }
    }

    // GET: Movies
    public async Task<IActionResult> Index()
    {
      return _context.Movies != null ?
                  View(await _context.Movies
                    .Include(m => m.Regions)
                    .ThenInclude(r => r.StreamingServices)
                    .Include(m => m.Cast)
                    .Include(m => m.Directors)
                    .Include(m => m.Genres)
                    .ToListAsync()) :
                  Problem("Entity set 'ApplicationDbContext.Movies'  is null.");
    }

    public async Task<IActionResult> Details(int? id)
    {
      if (id == null || _context.Movies == null)
      {
        return NotFound();
      }

      var movie = await _context.Movies
          .Include(m => m.Regions)
          .ThenInclude(r => r.StreamingServices)
          .Include(m => m.Cast)
          .Include(m => m.Directors)
          .Include(m => m.Genres)
          .FirstOrDefaultAsync(m => m.ID == id);

      if (movie == null)
      {
        return NotFound();
      }

      return View(movie);
    }

    public void SaveMovie(Show show)
    {
      if (show is RapidAPI.StreamingAvailability.Client.Models.Movie movie)
      {
        Models.Movie newMovie = new Models.Movie
        {
          Type = movie.Type,
          Title = movie.Title,
          BackdropPath = movie.BackdropPath,
          imdbId = movie.imdbId,
          imdbRating = movie.imdbRating,
          imdbVoteCount = movie.imdbVoteCount,
          OriginalLanguage = movie.OriginalLanguage,
          OriginalTitle = movie.OriginalTitle,
          PosterPath = movie.PosterPath,
          Overview = movie.Overview,
          ReleaseYear = movie.ReleaseYear,
          Runtime = movie.Runtime,
          Tagline = movie.Tagline,
          tmdbId = movie.tmdbId,
          tmdbRating = movie.tmdbRating,
          Trailer = movie.Trailer,
          TrailerID = movie.TrailerID,
          Cast = new List<Actor>(),
          Directors = new List<Director>(),
          Regions = new List<Models.RegionInfo>(),
          Genres = new List<Models.Genre>(),
        };

        if (movie.Cast.Count > 0)
        {
          foreach (var item in movie.Cast)
          {
            Actor actor = new()
            {
              Name = item.Name,
              MovieID = newMovie?.ID,
              Movie = newMovie,
            };

            newMovie?.Cast?.Add(actor);
          }
        }

        if (movie.Directors.Count > 0)
        {
          foreach (var item in movie.Directors)
          {
            Director director = new Director
            {
              Name = item.Name,
              MovieID = newMovie?.ID,
              Movie = newMovie,
            };

            newMovie?.Directors?.Add(director);
          }
        }

        if (movie.Regions.Count > 0)
        {
          foreach(var item in movie.Regions)
          {
            Models.RegionInfo region = new()
            {
              MovieID = newMovie?.ID,
              Movie = newMovie,
              RegionName = item.RegionName,
              StreamingServices = new List<Models.StreamingService>(),
            };

            if (item.StreamingServices.Count > 0)
            {
              foreach(var service in item.StreamingServices)
              {
                Models.StreamingService newService = new()
                {
                  AddOn = service.AddOn,
                  Link = service.Link,
                  Quality = service.Quality,
                  RegionInfo = region,
                  RegionInfoID = region?.RegionID,
                  Service = service.Service,
                  Type = service.Type,
                  watchLink = service.watchLink,
                  Audios = new List<Models.Audios>(),
                  Subtitles = new List<Models.Subtitles>()
                };

                if (service.Subtitles.Count > 0)
                {
                  foreach (var subtitle in service.Subtitles)
                  {
                    Models.Subtitles newSubtitles = new()
                    {
                      ClosedCaptions = subtitle.ClosedCaptions,
                      Language = subtitle.Language,
                      Region = subtitle.Region,
                      StreamingServiceID = newService?.ID,
                      StreamingService = newService
                    };
                    newService?.Subtitles?.Add(newSubtitles);
                  }
                }

                if (service.Audios.Count > 0)
                {
                  foreach (var audio in service.Audios)
                  {
                    Models.Audios newAudio = new()
                    {
                      Language = audio.Language,
                      Region = audio.Region,
                      StreamingServiceID = newService?.ID,
                    };
                    newAudio.StreamingService = newService;
                    newService?.Audios?.Add(newAudio);
                  }
                }

                region?.StreamingServices?.Add(newService);
              }
            }

            newMovie?.Regions?.Add(region);
          }
        }

        if (movie.Genres?.Count > 0)
        {
          foreach(var genres in movie.Genres)
          {
            Models.Genre genre = new()
            {
              Name = genres.Name,
              Movie = newMovie,
              MovieID = newMovie.ID
            };

            newMovie.Genres?.Add(genre);
          }
        }

        _context.Movies.Add(newMovie);
        _context.SaveChanges();
      }
    }
  }
}
