using System.Net;

namespace ReservaRussasAPI.Extensions
{
    public static class HttpExtensions
    {
        /// <summary>
        /// Validar se StatusCode está entre 200-299
        /// </summary>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public static bool IsSuccess(this HttpStatusCode statusCode) =>
            new HttpResponseMessage(statusCode).IsSuccessStatusCode;
    }
}
