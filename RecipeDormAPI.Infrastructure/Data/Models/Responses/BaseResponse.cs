using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeDormAPI.Infrastructure.Data.Models.Responses
{
    /*public class BaseResponse
    {
    }*/

    public class BaseResponse
    {
        public bool Status { get; set; }
        public string? Message { get; set; }

        public BaseResponse(bool status, string message)
        {
            Status = status;
            Message = message;
        }
    }

    public class BaseResponse<T>
    {
        public bool Status { get; set; }
        public string? Message { get; set; }

        public T? Data { get; set; }
        public BaseResponse()
        {

        }
        public BaseResponse(bool status, string message)
        {
            Status = status;
            Message = message;
        }
        public BaseResponse(bool status, string message, T data)
        {
            Status = status;
            Message = message;
            Data = data;
        }
    }
}
