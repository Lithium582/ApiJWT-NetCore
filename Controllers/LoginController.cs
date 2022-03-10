using JsonWebTokensAPI.Controllers.Token;
using JsonWebTokensAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Web.Http;

namespace JsonWebTokensAPI.Controllers
{
    /// <summary>
    /// Controlador para el login de los usuarios
    /// </summary>
    /// https://enmilocalfunciona.io/construyendo-una-web-api-rest-segura-con-json-web-token-en-net-parte-ii/
    [Microsoft.AspNetCore.Mvc.Route("~/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class LoginController : ControllerBase
    {
        [Microsoft.AspNetCore.Mvc.HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("echoping")]
        public IHttpActionResult EchoPing()
        {
            return (IHttpActionResult)Ok(true);
        }

        [Microsoft.AspNetCore.Mvc.HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("echouser")]
        public IHttpActionResult EchoUser()
        {
            var identity = Thread.CurrentPrincipal.Identity;
            return (IHttpActionResult)Ok($" IPrincipal-user: {identity.Name} - IsAuthenticated: {identity.IsAuthenticated}");
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("authenticate")]
        public IHttpActionResult Authenticate(LoginModel login)
        {
            if (login == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            //TODO: Validate credentials Correctly, this code is only for demo !!
            bool isCredentialValid = (login.Password == "123456");
            if (isCredentialValid)
            {
                var token = GeneradorToken.GenerarTokenJwt(login.Usuario);
                return (IHttpActionResult)Ok(token);
            }
            else
            {
                return (IHttpActionResult)Unauthorized();
            }
        }
    }
}
