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
                Add(movie);
            }
        }

        public void Add(Movie entity)
        {
            _movieRepository.Add(entity);
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

        public async Task<Movie?> SearchMovie(char letter)
        {
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.GetAsync($"http://www.omdbapi.com/?apikey=3eb9dfa5&s={letter}&plot=full");

            if (response.IsSuccessStatusCode)
            {
                var str = await response.Content.ReadAsStringAsync();
                dynamic searchData = JsonConvert.DeserializeObject(str);

                if (searchData != null && searchData.Search != null && searchData.Search.Count > 0)
                {
                    foreach (var searchDataItem in searchData.Search)
                    {
                        if (searchDataItem.Title.StartsWith(letter.ToString(), StringComparison.OrdinalIgnoreCase))
                        {
                            response = await httpClient.GetAsync($"http://www.omdbapi.com/?apikey=3eb9dfa5&t={searchDataItem.Title}&plot=full");

                            if (response.IsSuccessStatusCode)
                            {
                                str = await response.Content.ReadAsStringAsync();
                                dynamic singleData = JsonConvert.DeserializeObject(str);

                                var myMovie = new Movie
                                {
                                    About = singleData.Plot,
                                    ImagePath = singleData.Poster,
                                    MovieName = singleData.Title,
                                    Rating = singleData.imdbRating
                                };

                                return myMovie;
                            }
                        }
                    }
                }
            }
            return null; 
        }


        public void Update(Movie entity)
        {
            _movieRepository.Update(entity);
        }
    }
}
