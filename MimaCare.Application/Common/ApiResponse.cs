using System;
using System.Collections.Generic;
using System.Text;

namespace MimaCare.Application.Common
{
    public class ApiResponse<T>
    {
        public int StatusCode { get; set; }

        public string Message { get; set; }
            = default!;

        public T? Data { get; set; }

        public bool Success { get; set; }
    }
}
