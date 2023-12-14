using MovieTaskAspNet.Data;
using MovieTaskAspNet.Entities;
using MovieTaskAspNet.Repositories.Abstract;
using System.Linq.Expressions;

namespace MovieTaskAspNet.Repositories.Concrete
{
    public class MovieRepository : IMovieRepository
    {
        private readonly MovieDbContext _context;
        public void Add(Movie entity)
        {
            _context.Add(entity);
            _context.SaveChanges();
        }

        public void Delete(Movie entity)
        {
            _context.Remove(entity);
            _context.SaveChanges();
        }

        public Movie? Get(Expression<Func<Movie, bool>> expression)
        {
            var movie = _context.Movies.SingleOrDefault(expression);
            return movie;
        }

        public IEnumerable<Movie> GetAll()
        {
            var movies = _context.Movies;
            return movies;
        }

        public void Update(Movie entity)
        {
            _context.Movies.Update(entity);
            _context.SaveChanges();
        }
    }
}
