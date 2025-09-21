using System.Net;

namespace ReservaRussasAPI.Controllers.Base
{
    public class CustomResult
    {
        public HttpStatusCode StatusCode { get; private set; }
        public bool Success { get; private set; }
        public string Message { get; private set; }
        public string ApiVersion { get; private set; } 
        public DateTime ExecutedIn { get; private set; }  
        public object Data { get; private set; }
        public IEnumerable<string> Errors { get; private set; } 

        public CustomResult(HttpStatusCode statusCode, bool success)
        {
            StatusCode = statusCode;
            Success = success;
            ExecutedIn = DateTime.Now;
            Message = "API versão Teste BETA, Proibido o uso.";
            Errors = new List<string>();


        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="success"></param>
        /// <param name="data"></param>
        public CustomResult(HttpStatusCode statusCode, bool success, object data) : this(statusCode, success) =>
            Data = data;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="success"></param>
        /// <param name="errors"></param>
        public CustomResult(HttpStatusCode statusCode, bool success, IEnumerable<string> errors) : this(statusCode, success) =>
            Errors = errors;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="success"></param>
        /// <param name="data"></param>
        /// <param name="errors"></param>
        public CustomResult(HttpStatusCode statusCode, bool success, object data, IEnumerable<string> errors) : this(statusCode, success, data) =>
            Errors = errors;
    }
}
