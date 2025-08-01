using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductWeb.models
{
    public class ApiResponse<T>
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }

}