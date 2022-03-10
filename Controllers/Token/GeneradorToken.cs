using System;
using System.Configuration;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace JsonWebTokensAPI.Controllers.Token
{
    /// <summary>
    /// Clase generadora de tokens JWT usando "secret-key"
    /// </summary>
    internal static class GeneradorToken
    {
        private static IConfiguration configuracion;

        public static string GenerarTokenJwt(string usuario)
        {
            // TODO: Actualizar cómo se levantan los datos del appsettings.json (está pensado para un Web Config)
            // Valores almacenados en appsettings.json para generar el Token JWT
            var secretKey = configuracion["JWT_SECRET_KEY"];  //ConfigurationManager.AppSettings["JWT_SECRET_KEY"];
            var audienceToken = configuracion["JWT_AUDIENCE_TOKEN"]; //ConfigurationManager.AppSettings["JWT_AUDIENCE_TOKEN"];
            var issuerToken = configuracion["JWT_ISSUER_TOKEN"]; //ConfigurationManager.AppSettings["JWT_ISSUER_TOKEN"];
            var expireTime = configuracion["JWT_EXPIRE_MINUTES"]; //ConfigurationManager.AppSettings["JWT_EXPIRE_MINUTES"];

            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(secretKey));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            // create a claimsIdentity
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, usuario) });

            // create token to the user
            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwtSecurityToken = tokenHandler.CreateJwtSecurityToken(
                audience: audienceToken,
                issuer: issuerToken,
                subject: claimsIdentity,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(expireTime)),
                signingCredentials: signingCredentials);

            var jwtTokenString = tokenHandler.WriteToken(jwtSecurityToken);
            return jwtTokenString;
        }
    }
}
