using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MovieTaskAspNet.Entities;
using MovieTaskAspNet.Repositories.Abstract;
using MovieTaskAspNet.Services.Abstract;
using Newtonsoft.Json;

namespace MovieTaskAspNet.Services.Concrete
{
    public class MovieService : IMovieService
    {
        private readonly IConfiguration _configuration;
        private readonly IMovieRepository _movieRepository;
        private readonly BackgroundWorker _backgroundWorker;
        private bool _isWorking;

        public static dynamic? Data { get; set; }
        public static dynamic? SingleData { get; set; }

        static Random _random = new Random();

        public MovieService(IMovieRepository movieRepository,IConfiguration configuration)
        {
            _movieRepository = movieRepository;
            _configuration = configuration;
            _backgroundWorker = new BackgroundWorker();
            _backgroundWorker.DoWork += BackgroundWorker_DoWork;

            _backgroundWorker.RunWorkerAsync();
        }

        private void BackgroundWorker_DoWork(object? sender, DoWorkEventArgs e)
        {
            BackgroundWorker? worker = sender as BackgroundWorker;

            while (!worker.CancellationPending)
            {
                if (!_isWorking)
                {
                    var word = GetLetter();
                    _isWorking = true;
                    SearchAndSaveMovie(word);
                    _isWorking = false;
                }
                int sleepTime = _configuration.GetValue<int>("AppSettings:SleepTime");
                Thread.Sleep(sleepTime*1000);
            }
        }

        public char GetLetter()
        {
            int num = _random.Next(0, 26);
            char let = (char)('a' + num);
            return let;
        }

        public async void SearchAndSaveMovie(char letter)
        {
            var movie = await SearchMovie(letter);
            if (movie != null)
            {
                await Add(movie);
            }
        }

        public Task Add(Movie entity)
        {
            _movieRepository.Add(entity);
            return Task.CompletedTask;
        }

        public void Delete(int id)
        {
            var item = _movieRepository.Get(x => x.Id == id);
            _movieRepository.Delete(item);
        }

        public Movie Get(Expression<Func<Movie, bool>> expression)
        {
            return _movieRepository.Get(expression);
        }

        public IEnumerable<Movie> GetAll()
        {
            return _movieRepository.GetAll();
        }
        //public async Task<Movie> GetMovieFromApi(string letter)
        //{
        //    HttpClient httpClient = new HttpClient();

        //    string apiKey = "3eb9dfa5";
        //    string apiUrl = $"https://www.omdbapi.com/?apikey={apiKey}&t={letter}*";

        //    var response = await httpClient.GetAsync(apiUrl);

        //    if (response.IsSuccessStatusCode)
        //    {
        //        var content = await response.Content.ReadAsStringAsync();
        //        dynamic singleData = JsonConvert.DeserializeObject(content);

        //        var myMovie = new Movie
        //        {
        //            About = singleData.Plot,
        //            ImagePath = singleData.Poster,
        //            MovieName = singleData.Title,
        //            Rating = singleData.imdbRating
        //        };
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}
        public async Task<Movie?> SearchMovie(char letter)
        {
            HttpClient httpClient = new HttpClient();

            string apiKey = "3eb9dfa5";
            string apiUrl = $"https://www.omdbapi.com/?apikey={apiKey}&t={letter}*";

            var response = await httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                dynamic? singleData = JsonConvert.DeserializeObject(content);

                var myMovie = new Movie
                {
                    MovieName = singleData?.Title
                };
                return myMovie;
            }
            else
            {
                return null;
            }
        }


        public void Update(Movie entity)
        {
            _movieRepository.Update(entity);
        }
    }
}
