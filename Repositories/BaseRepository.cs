
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Repositories
{
    public abstract class BaseRepository<T> where T : class
    {
        private readonly DorakContext dorakContext;
    }
}
