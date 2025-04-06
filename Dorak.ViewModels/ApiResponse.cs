using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dorak.ViewModels
{
    public class ApiResponse<T>
    {
        public string Message {  get; set; }
        public int Status { get; set; }
        public T Data { get; set; }
    }
}
