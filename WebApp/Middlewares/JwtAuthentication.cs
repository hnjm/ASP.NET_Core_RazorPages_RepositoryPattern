using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebApp.Middlewares
{
    public class JWTAuthentication : IJwtAuthentication
    {
        private readonly IConfiguration _configuration;

        public JWTAuthentication(IConfiguration configuration)
        {
            this._configuration = configuration;    
        }
        
        public string GenerateJwtToken(string username, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sid, username),
                new Claim(ClaimTypes.NameIdentifier, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())                
            };
            
            claims.Add(new Claim(ClaimTypes.Role, role));            

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Convert.ToString(_configuration["JwtConfig:key"])));
            var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(Convert.ToString(_configuration["JwtConfig:expiredays"])));

            var token = new JwtSecurityToken(
                Convert.ToString(_configuration["JwtConfig:issuer"]),
                Convert.ToString(_configuration["JwtConfig:audience"]),
                claims,
                expires: expires,
                signingCredentials: credential
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        
        public string ValidateToken(string token)
        {
            if (token == null)
                return null;

            var handler = new JwtSecurityTokenHandler();
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtConfig:key"]));
            try
            {
                handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = securityKey,                    
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = Convert.ToString(_configuration["JwtConfig:issuer"]),
                    ValidAudience = Convert.ToString(_configuration["JwtConfig:audience"])
                }, out SecurityToken validToken);

                var jwtToken = (JwtSecurityToken)validToken;
                
                var userName = jwtToken.Claims.First(sub => sub.Type == "sid").Value;                
                return userName;
            }
            catch
            {                
                return null;
            }
        }
    }
}
