using MovieTaskAspNet.Entities;
using MovieTaskAspNet.Repositories.Abstract;
using MovieTaskAspNet.Repositories.Concrete;
using MovieTaskAspNet.Services.Abstract;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Linq.Expressions;

namespace MovieTaskAspNet.Services.Concrete
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository; 
        private BackgroundWorker _backgroundWorker;
        private bool isWorking;
        public static dynamic? Data { get; set; }
        public static dynamic? SingleData { get; set; }
        static Random random = new Random();
        public static char GetLetter()
        {
            int num = random.Next(0, 26);
            char let = (char)('a' + num);
            return let;
        }
        public MovieService(IMovieRepository movieRepository,BackgroundWorker backgroundWorker)
        {
            _movieRepository = movieRepository;
            _backgroundWorker = backgroundWorker;
            InitializeBackWorker();
        }
        public void InitializeBackWorker()
        {
            _backgroundWorker = new BackgroundWorker();
            _backgroundWorker.DoWork += BackgroundWorker_DoWork;
        }
        
        private void BackgroundWorker_DoWork(object? sender, DoWorkEventArgs e)
        {
            BackgroundWorker? worker = sender as BackgroundWorker;
            while (!worker.CancellationPending && !isWorking)
            {
                var word = GetLetter();
                isWorking = true;
                SearchMovie(word);

                Thread.Sleep(10000);
                isWorking = !isWorking;
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

        public Movie SearchMovie(char letter)
        {
            
            HttpClient httpClient = new HttpClient();
                HttpResponseMessage response = new HttpResponseMessage();

                response = httpClient.GetAsync($@"http://www.omdbapi.com/?apikey=3eb9dfa5&s={letter}&plot=full").Result;
                var str = response.Content.ReadAsStringAsync().Result;
                Data = JsonConvert.DeserializeObject(str);

                List<Movie> movies = new List<Movie>();
                var myMovie = new Movie { };
            try
            {
                for (int i = 0; i < 100; i++)
                {
                    response = httpClient.GetAsync($@"http://www.omdbapi.com/?apikey=3eb9dfa5&t={Data.Search[i].Title}&plot=full").Result;
                    str = response.Content.ReadAsStringAsync().Result;
                    SingleData = JsonConvert.DeserializeObject(str);
                    myMovie = new Movie
                    {
                        About = SingleData.Plot,
                        ImagePath = SingleData.Poster,
                        MovieName = SingleData.Title,
                        Rating = SingleData.imdbRating,
                    };
                    _movieRepository.Get(m => m.MovieName == myMovie.MovieName);
                    if (_movieRepository == null)
                    {
                        break;
                    }
                   
                }
            }
            catch (Exception)
            {
            }
            return myMovie;
        }

        public void Update(Movie entity)
        {
            _movieRepository.Update(entity);    
        }
    }
}
