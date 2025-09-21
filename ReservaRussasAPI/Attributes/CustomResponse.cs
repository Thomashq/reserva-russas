using Microsoft.AspNetCore.Mvc;
using ReservaRussasAPI.Controllers.Base;

namespace ReservaRussasAPI.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class CustomResponseAttribute : ProducesResponseTypeAttribute
    {
        /// <summary>
        /// Resposta Customizada padronizada para todas as Restposta com código injetado 
        /// o CustomResult será sempre um padrão de Resposta para as API
        /// </summary>
        /// <param name="statusCode"></param>
        public CustomResponseAttribute(int statusCode) : base(typeof(CustomResult), statusCode) { }
    }
}
