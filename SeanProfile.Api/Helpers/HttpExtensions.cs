using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Globalization;
using System.Text;

namespace SeanProfile.Helpers
{
    public class ValidationError : IActionResult
    {
        private readonly string _message;

        public ValidationError(string message)
        {
            _message = message;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var response = context.HttpContext.Response;
            response.ContentType = "application/json";
            response.StatusCode = 299;
            using (var mStrm = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(_message))))
            {
                await mStrm.CopyToAsync(context.HttpContext.Response.Body);
            }
        }
    }

    public class AppException : Exception
    {
        public AppException() : base() { }

        public AppException(string message) : base(message) { }

        public AppException(string message, params object[] args)
            : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }

}

