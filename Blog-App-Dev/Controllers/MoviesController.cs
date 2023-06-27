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

namespace Blog_App_Dev.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult SearchMovie(string title)
        {
            var client = new RestClient("https://streaming-availability.p.rapidapi.com/v2/search/title?title=" + title + "&country=us&show_type=movie&output_language=en");
            var request = new RestRequest();
            request.AddHeader("X-RapidAPI-Key", "346e77a6f3msh163186a7d97775fp1d3a0djsn807dde9d0dce");
            request.AddHeader("X-RapidAPI-Host", "streaming-availability.p.rapidapi.com");
            RestResponse response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                JObject json = JObject.Parse(response.Content);
                foreach (var (movieData, movie) in from movieData in json.GetValue("result")
                                                   let movie = new Movie
                                                   {
                                                       Type = movieData.GetValue<string>("type"),
                                                       Title = movieData.GetValue<string>("title"),
                                                       Overview = movieData.GetValue<string>("overview"),
                                                       Regions = new List<RegionInfo>(),
                                                       Cast = new List<Actor>(),
                                                       Directors = new List<Director>(),
                                                       ReleaseYear = movieData.GetValue<int>("year"),
                                                       imdbId = movieData.GetValue<string>("imdbId"),
                                                       imdbRating = movieData.GetValue<int>("imdbRating"),
                                                       imdbVoteCount = movieData.GetValue<int>("imdbVoteCount"),
                                                       tmdbId = movieData.GetValue<int>("tmdbId"),
                                                       tmdbRating = movieData.GetValue<int>("tmdbRating"),
                                                       OriginalTitle = movieData.GetValue<string>("originalTitle"),
                                                       BackdropPath = movieData.GetValue<string>("backdropPath"),
                                                       Runtime = movieData.GetValue<int>("runtime"),
                                                       Trailer = movieData.GetValue<string>("youtubeTrailerVideoLink"),
                                                       TrailerID = movieData.GetValue<string>("youtubeTrailerVideoId"),
                                                       PosterPath = movieData.GetValue<string>("posterPath"),
                                                       Tagline = movieData.GetValue<string>("tagline")
                                                   }
                                                   select (movieData, movie))
                {
                    /* Getting all the streaming services and the info from them */
                    foreach (KeyValuePair<string, JToken> region in movieData["streamingInfo"].ToObject<JObject>())
                    {
                        var regionInfo = new RegionInfo
                        {
                            RegionName = region.Key,
                            StreamingServices = new List<StreamingService>(),
                            MovieID = movie?.ID,
                            Movie = movie
                        };

                        foreach (KeyValuePair<string, JToken> service in region.Value.ToObject<JObject>())
                        {
                            foreach (JObject info in service.Value.ToObject<JArray>())
                            {
                                var streamingService = new StreamingService
                                {
                                    Service = service.Key,
                                    Type = info.GetValue<string>("type"),
                                    Quality = info.GetValue<string>("quality"),
                                    AddOn = info.GetValue<string>("addOn"),
                                    Link = info.GetValue<string>("link"),
                                    watchLink = info.GetValue<string>("watchLink"),
                                    Audios = new List<Audios>(),
                                    Subtitles = new List<Subtitles>(),
                                    RegionInfoID = regionInfo.RegionID,
                                    RegionInfo = regionInfo
                                };

                                if (info.TryGetValue("audios", out JToken? audiosList))
                                {
                                    foreach (JObject audio in audiosList.ToObject<JArray>())
                                    {
                                        var audios = new Audios
                                        {
                                            Language = audio.GetValue<string>("language"),
                                            Region = audio.GetValue<string>("region"),
                                            StreamingServiceID = streamingService.ID,
                                            StreamingService = streamingService
                                        };
                                        streamingService.Audios.Add(audios);
                                    }
                                }

                                if (info.TryGetValue("subtitles", out JToken? subtitlesList))
                                {
                                    foreach (JObject subtitle in subtitlesList.ToObject<JArray>())
                                    {
                                        var subtitles = new Subtitles
                                        {
                                            Language = subtitle["locale"].ToObject<JObject>().GetValue<string>("language"),
                                            Region = subtitle["locale"].ToObject<JObject>().GetValue<string>("region"),
                                            ClosedCaptions = subtitle["locale"].ToObject<JObject>().GetValue<bool>("closedCaptions"),
                                            StreamingServiceID = streamingService.ID,
                                            StreamingService = streamingService
                                        };
                                        streamingService.Subtitles.Add(subtitles);
                                    }
                                }

                                regionInfo.StreamingServices.Add(streamingService);
                            }
                        }

                        movie?.Regions.Add(regionInfo);
                    }
                    /* Add all Generas, actors and directors */
                    JToken cast = movieData["cast"];
                    if (cast != null)
                    {
                        foreach (var person in cast)
                        {
                            var actor = new Actor
                            {
                                Name = (string)person,
                                MovieID = movie?.ID,
                                Movie = movie
                            };
                            movie?.Cast.Add(actor);
                        }
                    }

                    JToken directors = movieData["directors"];
                    if (directors != null)
                    {
                        foreach (var person in directors)
                        {
                            var director = new Director
                            {
                                Name = (string)person,
                                MovieID = movie?.ID,
                                Movie = movie
                            };
                            movie?.Directors.Add(director);
                        }
                    }

                    JToken genresList = movieData["genres"];
                    if (genresList != null)
                    {
                        movie.Genres = new List<Genre>();

                        foreach (JObject genre in genresList.ToObject<JArray>())
                        {
                            var newGenre = new Genre
                            {
                                Name = genre.GetValue<string>("name"),
                                MovieID = movie.ID,
                                Movie = movie,
                            };

                            movie.Genres.Add(newGenre);
                        }
                    }

                    _context.Movies.Add(movie);
                    _context.SaveChanges();
                }

                return RedirectToAction("Index");
            }

            return View("Error"); // return an error view if the API call was not successful
        }

        // GET: Movies
        public async Task<IActionResult> Index()
        {
              return _context.Movies != null ? 
                          View(await _context.Movies.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Movies'  is null.");
        }
    }
}
