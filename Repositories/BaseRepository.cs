
using Data;

namespace Repositories
{
    public abstract class BaseRepository<T> where T : class
    {
        private readonly DorakContext dorakContext;
    }
}
