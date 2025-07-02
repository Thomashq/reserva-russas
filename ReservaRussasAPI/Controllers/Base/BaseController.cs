using Microsoft.AspNetCore.Mvc;
using RR.Core.Common;
using RR.Core.Extensions;
using System.Net;

namespace ReservaRussasAPI.Controllers.Base
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public abstract class BaseControllerFYP : ControllerBase
    {
        /// <summary>
        /// Response OK com dados
        /// </summary>
        protected IActionResult ResponseOk<T>(T data, string? message = null) =>
            CreateResponse(HttpStatusCode.OK, data, message);

        /// <summary>
        /// Response OK sem dados
        /// </summary>
        protected IActionResult ResponseOk(string? message = null) =>
            CreateResponse<object>(HttpStatusCode.OK, null, message);

        /// <summary>
        /// Response Created com dados
        /// </summary>
        protected IActionResult ResponseCreated<T>(T data, string? message = null) =>
            CreateResponse(HttpStatusCode.Created, data, message);

        /// <summary>
        /// Response Created sem dados
        /// </summary>
        protected IActionResult ResponseCreated(string? message = null) =>
            CreateResponse<object>(HttpStatusCode.Created, null, message);

        /// <summary>
        /// Response No Content
        /// </summary>
        protected IActionResult ResponseNoContent(string? message = null) =>
            CreateResponse<object>(HttpStatusCode.NoContent, null, message);

        /// <summary>
        /// Response Bad Request
        /// </summary>
        protected IActionResult ResponseBadRequest(string errorMessage) =>
            CreateResponse<object>(HttpStatusCode.BadRequest, null, null, new List<string> { errorMessage });

        /// <summary>
        /// Response Bad Request com múltiplos erros
        /// </summary>
        protected IActionResult ResponseBadRequest(List<string> errors) =>
            CreateResponse<object>(HttpStatusCode.BadRequest, null, null, errors);

        /// <summary>
        /// Response Bad Request padrão
        /// </summary>
        protected IActionResult ResponseBadRequest() =>
            ResponseBadRequest("A requisição é inválida");

        /// <summary>
        /// Response Not Found
        /// </summary>
        protected IActionResult ResponseNotFound(string errorMessage) =>
            CreateResponse<object>(HttpStatusCode.NotFound, null, null, new List<string> { errorMessage });

        /// <summary>
        /// Response Not Found padrão
        /// </summary>
        protected IActionResult ResponseNotFound() =>
            ResponseNotFound("O recurso não foi encontrado");

        /// <summary>
        /// Response Unauthorized
        /// </summary>
        protected IActionResult ResponseUnauthorized(string errorMessage) =>
            CreateResponse<object>(HttpStatusCode.Unauthorized, null, null, new List<string> { errorMessage });

        /// <summary>
        /// Response Unauthorized padrão
        /// </summary>
        protected IActionResult ResponseUnauthorized() =>
            ResponseUnauthorized("Permissão negada");

        /// <summary>
        /// Response Internal Server Error
        /// </summary>
        protected IActionResult ResponseInternalServerError(string errorMessage) =>
            CreateResponse<object>(HttpStatusCode.InternalServerError, null, null, new List<string> { errorMessage });

        /// <summary>
        /// Response Internal Server Error com Exception
        /// </summary>
        protected IActionResult ResponseInternalServerError(Exception exception) =>
            ResponseInternalServerError(exception.Message);

        /// <summary>
        /// Response Internal Server Error padrão
        /// </summary>
        protected IActionResult ResponseInternalServerError() =>
            ResponseInternalServerError("Erro interno do servidor");

        /// <summary>
        /// Cria resposta customizada
        /// </summary>
        private JsonResult CreateResponse<T>(HttpStatusCode statusCode, T? data = default, string? message = null, List<string>? errors = null)
        {
            var success = statusCode.IsSuccess();
            var response = new ApiResponse<T>
            {
                StatusCode = statusCode,
                Success = success,
                Data = data,
                Message = message,
                Errors = errors ?? new List<string>()
            };

            return new JsonResult(response)
            {
                StatusCode = (int)statusCode
            };
        }

        /// <summary>
        /// Valida ModelState e retorna erros se inválido
        /// </summary>
        protected IActionResult? ValidateModelState()
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .SelectMany(x => x.Value?.Errors ?? new Microsoft.AspNetCore.Mvc.ModelBinding.ModelErrorCollection())
                    .Select(x => x.ErrorMessage)
                    .ToList();

                return ResponseBadRequest(errors);
            }
            return null;
        }
    }
}
