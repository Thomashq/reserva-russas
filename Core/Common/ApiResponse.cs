using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RR.Core.Common
{
    /// <summary>
    /// Resposta padrão da API
    /// </summary>
    /// <typeparam name="T">Tipo dos dados retornados</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Código de status HTTP
        /// </summary>
        [JsonPropertyName("statusCode")]
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Indica se a operação foi bem-sucedida
        /// </summary>
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        /// <summary>
        /// Dados retornados pela API
        /// </summary>
        [JsonPropertyName("data")]
        public T? Data { get; set; }

        /// <summary>
        /// Mensagem adicional (opcional)
        /// </summary>
        [JsonPropertyName("message")]
        public string? Message { get; set; }

        /// <summary>
        /// Lista de erros (se houver)
        /// </summary>
        [JsonPropertyName("errors")]
        public List<string> Errors { get; set; } = new();

        /// <summary>
        /// Timestamp da resposta
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Construtor padrão
        /// </summary>
        public ApiResponse()
        {
        }

        /// <summary>
        /// Construtor para resposta de sucesso com dados
        /// </summary>
        /// <param name="data">Dados a serem retornados</param>
        /// <param name="message">Mensagem opcional</param>
        public ApiResponse(T data, string? message = null)
        {
            StatusCode = HttpStatusCode.OK;
            Success = true;
            Data = data;
            Message = message;
        }

        /// <summary>
        /// Construtor para resposta de erro
        /// </summary>
        /// <param name="statusCode">Código de status HTTP</param>
        /// <param name="errors">Lista de erros</param>
        public ApiResponse(HttpStatusCode statusCode, List<string> errors)
        {
            StatusCode = statusCode;
            Success = false;
            Errors = errors ?? new List<string>();
        }

        /// <summary>
        /// Construtor para resposta de erro com uma mensagem
        /// </summary>
        /// <param name="statusCode">Código de status HTTP</param>
        /// <param name="error">Mensagem de erro</param>
        public ApiResponse(HttpStatusCode statusCode, string error)
        {
            StatusCode = statusCode;
            Success = false;
            Errors = new List<string> { error };
        }
    }

    /// <summary>
    /// Resposta da API sem dados tipados
    /// </summary>
    public class ApiResponse : ApiResponse<object>
    {
        public ApiResponse() : base() { }
        public ApiResponse(string? message = null) : base(null, message) { }
        public ApiResponse(HttpStatusCode statusCode, List<string> errors) : base(statusCode, errors) { }
        public ApiResponse(HttpStatusCode statusCode, string error) : base(statusCode, error) { }
    }
}
