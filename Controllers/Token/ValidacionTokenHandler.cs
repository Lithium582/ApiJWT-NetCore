using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.IdentityModel.Tokens;

namespace JsonWebTokensAPI.Controllers.Token
{
    /// <summary>
    /// Validador del Token para autorizar requests usando DelegatingHandler
    /// </summary>s
    internal class ValidacionTokenHandler : DelegatingHandler
    {
        private IConfiguration configuracion;

        private static bool TryObtenerToken(HttpRequestMessage request, out string token)
        {
            token = null;
            IEnumerable<string> authzHeaders;

            if (!request.Headers.TryGetValues("Authorization", out authzHeaders) || authzHeaders.Count() > 1)
            {
                return false;
            }

            var bearerToken = authzHeaders.ElementAt(0);
            token = bearerToken.StartsWith("Bearer ") ? bearerToken.Substring(7) : bearerToken;

            return true;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpStatusCode statusCode;
            string token;

            // determine whether a jwt exists or not
            if (!TryObtenerToken(request, out token))
            {
                statusCode = HttpStatusCode.Unauthorized;
                return base.SendAsync(request, cancellationToken);
            }

            // TODO: Actualizar cómo se levantan los datos del appsettings.json (está pensado para un Web Config)
            try
            {
                var secretKey = configuracion["JWT_SECRET_KEY"]; // ConfigurationManager.AppSettings["JWT_SECRET_KEY"];
                var audienceToken = configuracion["JWT_AUDIENCE_TOKEN"]; // ConfigurationManager.AppSettings["JWT_AUDIENCE_TOKEN"];
                var issuerToken = configuracion["JWT_ISSUER_TOKEN"]; // ConfigurationManager.AppSettings["JWT_ISSUER_TOKEN"];

                var securityKey = new SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(secretKey));

                SecurityToken securityToken;
                var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();

                TokenValidationParameters validationParameters = new TokenValidationParameters()
                {
                    ValidAudience = audienceToken,
                    ValidIssuer = issuerToken,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    LifetimeValidator = this.ValidadorTiempoDeVida,
                    IssuerSigningKey = securityKey
                };

                // Extract and assign Current Principal and user
                //TODO: Chequear qué tan necesario es hacer esto
                Thread.CurrentPrincipal = tokenHandler.ValidateToken(token, validationParameters, out securityToken);

                //TODO: Chequear qué tan necesario es hacer esto
                //Comentado porque en NET Core esto no funciona
                //HttpContext.Current.User = tokenHandler.ValidateToken(token, validationParameters, out securityToken);

                return base.SendAsync(request, cancellationToken);
            }
            catch (SecurityTokenValidationException)
            {
                statusCode = HttpStatusCode.Unauthorized;
            }
            catch (Exception)
            {
                statusCode = HttpStatusCode.InternalServerError;
            }

            return Task<HttpResponseMessage>.Factory.StartNew(() => new HttpResponseMessage(statusCode) { });
        }

        public bool ValidadorTiempoDeVida(DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters)
        {
            if (expires != null)
            {
                if (DateTime.UtcNow < expires) return true;
            }
            return false;
        }
    }
}
