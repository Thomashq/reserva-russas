using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservaRussasAPI.Extensions;

using System.Net;

namespace ReservaRussasAPI.Controllers.Base
{
    [ApiVersion("1", Deprecated = true)]
    [ApiController]
    [Route("api/v{version:ApiVersion}/[controller]")]
    [Authorize]
    public abstract class BaseControllerFYP : ControllerBase, IDisposable
    {
        private bool disposedValue;

        protected IActionResult ResponseOk(object result) =>
            Response(HttpStatusCode.OK, result);

        protected IActionResult ResponseOk() =>
            Response(HttpStatusCode.OK);

        protected IActionResult ResponseCreated() =>
            Response(HttpStatusCode.Created);
   
        protected IActionResult ResponseCreated(object data) =>
            Response(HttpStatusCode.Created, data);

        protected IActionResult ResponseNoContent() =>
            Response(HttpStatusCode.NoContent);

        protected IActionResult ResponseNotModified(string? msg) =>
            Response(HttpStatusCode.NotModified, msg);

        protected IActionResult ResponseBadRequest(string errorMessage) =>
            Response(HttpStatusCode.BadRequest, errorMessage: errorMessage);

        protected IActionResult ResponseBadRequest() =>
            Response(HttpStatusCode.BadRequest, errorMessage: "A requisição é inválida");


        protected IActionResult ResponseNotFound(string errorMessage) =>
            Response(HttpStatusCode.NotFound, errorMessage: errorMessage);

        protected IActionResult ResponseNotFound() =>
            Response(HttpStatusCode.NotFound, errorMessage: "O recurso não foi encontrado");

        protected IActionResult ResponseUnauthorized(string errorMessage) =>
            Response(HttpStatusCode.Unauthorized, errorMessage: errorMessage);

        protected IActionResult ResponseUnauthorized() =>
            Response(HttpStatusCode.Unauthorized, errorMessage: "Permissão negada");

        protected IActionResult ResponseInternalServerError() =>
            Response(HttpStatusCode.InternalServerError);

        protected IActionResult ResponseInternalServerError(string errorMessage) =>
            Response(HttpStatusCode.InternalServerError, errorMessage: errorMessage);

        protected IActionResult ResponseInternalServerError(Exception exception) =>
            Response(HttpStatusCode.InternalServerError, errorMessage: exception.Message);

        protected new JsonResult Response(string uri, HttpStatusCode statusCode, object data, string errorMessage)
        {
            CustomResult result = null;

            if (string.IsNullOrWhiteSpace(errorMessage))
            {
                var success = statusCode.IsSuccess();

                if (data != null)
                    result = new CustomResult(statusCode, success, data);
                else
                    result = new CustomResult(statusCode, success);
            }
            else
            {
                var errors = new List<string>();

                if (!string.IsNullOrWhiteSpace(errorMessage))
                    errors.Add(errorMessage);

                result = new CustomResult(statusCode, false, errors);
            }
            return new JsonResult(result) { StatusCode = (int)result.StatusCode };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        protected new JsonResult Response(HttpStatusCode statusCode, object result) =>
            Response(null, statusCode, result, null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        protected new JsonResult Response(HttpStatusCode statusCode, string errorMessage) =>
            Response(null, statusCode, null, errorMessage);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        protected new JsonResult Response(HttpStatusCode statusCode) =>
            Response(null, statusCode, null, null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="statusCode"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        protected new JsonResult Response(string uri, HttpStatusCode statusCode, object result) =>
            Response(uri, statusCode, null);

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                   
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

    }
}
