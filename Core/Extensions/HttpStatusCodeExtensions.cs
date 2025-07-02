using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RR.Core.Extensions
{
    public static class HttpStatusCodeExtensions
    {
        /// <summary>
        /// Verifica se o status code representa sucesso (2xx)
        /// </summary>
        /// <param name="statusCode">O status code a ser verificado</param>
        /// <returns>True se for um status de sucesso, false caso contrário</returns>
        public static bool IsSuccess(this HttpStatusCode statusCode)
        {
            var code = (int)statusCode;
            return code >= 200 && code < 300;
        }

        /// <summary>
        /// Verifica se o status code representa erro do cliente (4xx)
        /// </summary>
        /// <param name="statusCode">O status code a ser verificado</param>
        /// <returns>True se for um erro do cliente, false caso contrário</returns>
        public static bool IsClientError(this HttpStatusCode statusCode)
        {
            var code = (int)statusCode;
            return code >= 400 && code < 500;
        }

        /// <summary>
        /// Verifica se o status code representa erro do servidor (5xx)
        /// </summary>
        /// <param name="statusCode">O status code a ser verificado</param>
        /// <returns>True se for um erro do servidor, false caso contrário</returns>
        public static bool IsServerError(this HttpStatusCode statusCode)
        {
            var code = (int)statusCode;
            return code >= 500 && code < 600;
        }
    }
 }
