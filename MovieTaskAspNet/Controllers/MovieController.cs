using Microsoft.AspNetCore.Mvc;
using MovieTaskAspNet.Entities;
using MovieTaskAspNet.Services.Abstract;

namespace MovieTaskAspNet.Controllers
{
    public class MovieController:ControllerBase
    {
        private IMovieService _movieService;

        public MovieController(IMovieService movieService)
        {
            _movieService = movieService;
        }
        [HttpGet]
        public IEnumerable<Movie> Get()
        {
            var items=_movieService.GetAll();
            var dataToReturn = items.Select(m =>
            {
                return new Movie
                {
                    Id = m.Id,
                    MovieName = m.MovieName,
                     About=m.About,
                      ImagePath = m.ImagePath,
                      Rating = m.Rating
                };
            });
            return dataToReturn;
        }
        [HttpGet("{id}")]
        public Movie Get(int id)
        {
            var item = _movieService.Get(m => m.Id == id);
            return new Movie
            {
               Id= item.Id,
               MovieName = item.MovieName,
                About = item.About,
                 Rating= item.Rating,
                  ImagePath=item.ImagePath
            };
        }
        [HttpPost]
        public IActionResult Post([FromBody] Movie value)
        {
            try
            {
                var entity = new Movie
                {
                    Id = value.Id,
                    MovieName = value.MovieName,
                    About = value.About,
                    Rating = value.Rating,
                    ImagePath = value.ImagePath
                };
                _movieService.Add(entity);
                return Ok(entity);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
