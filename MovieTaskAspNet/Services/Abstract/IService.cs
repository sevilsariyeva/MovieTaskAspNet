using System.Linq.Expressions;

namespace MovieTaskAspNet.Services.Abstract
{
    public interface IService<T>
    {
        IEnumerable<T> GetAll();
        T Get(Expression<Func<T, bool>> expression);
        void Add(T entity);
        void Update(T entity);
        void Delete(int id);
        T SearchMovie(char letter);
    }
}
