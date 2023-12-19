using MovieTaskAspNet.Entities;
using System.Linq.Expressions;

namespace MovieTaskAspNet.Services.Abstract
{
    public interface IService<T>
    {
        IEnumerable<T> GetAll();
        T Get(Expression<Func<T, bool>> expression);
        Task Add(T entity);
        void Update(T entity);
        void Delete(int id);
        Task<Movie> SearchMovie(char letter);
    }
}
