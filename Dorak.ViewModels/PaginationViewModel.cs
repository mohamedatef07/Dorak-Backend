using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dorak.ViewModels
{
    public class PaginationViewModel<T>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 16;
        public int Total { get; set; } = 20;
        public List<T> Data { get; set; }
    }
}