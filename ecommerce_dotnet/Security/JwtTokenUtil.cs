using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using quest_web.Models;
namespace quest_web.Security
{
    public interface IJwtUtils
    {
        public string GenerateToken(UserDetails user);
        public string GenerateToken(Claim[] claims);
        public (ClaimsPrincipal, JwtSecurityToken) DecodeToken(string token);
        public DateTime GetExpirationDateFromToken(string token);
        public string GetUsernameFromToken(string token);
        public bool IsTokenExpired(string token);
        public bool ValidateToken(string token, UserDetails details);

    }
    public class JwtTokenUtil : IJwtUtils
    {
        public static readonly long JwtTokenValidity = 5 * 360; 
        private static readonly string Secret = "etna_quest_jwt_secret_key"; 
        public static readonly TokenValidationParameters TokenValidationParameters = new()
        { 
            ValidateAudience = false, 
            ValidateIssuer = false, 
            ValidateIssuerSigningKey = true, 
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret)), ValidateLifetime = true, 
        }; 
    
        public string GenerateToken(UserDetails user) { 
            var claims = new[] { 
                new Claim(ClaimTypes.Name, user.Username), 
                new Claim(ClaimTypes.Role, user.Role.ToString()), 
            }; 
            return GenerateToken(claims); 
        }
        public string GenerateToken(Claim[] claims) { 
            var jwtToken = new JwtSecurityToken(
                claims: claims, 
                expires: DateTime.Now.AddSeconds(JwtTokenValidity), 
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret)), 
                    SecurityAlgorithms.HmacSha512Signature)
                ); 
            return new JwtSecurityTokenHandler().WriteToken(jwtToken); 
        }
        public (ClaimsPrincipal, JwtSecurityToken) DecodeToken(string token) { 
            var principal = new JwtSecurityTokenHandler().ValidateToken(
                token, 
                TokenValidationParameters, 
                out var validatedToken); 
            return (principal, validatedToken as JwtSecurityToken); 
        }
        public DateTime GetExpirationDateFromToken(string token) { 
            var (_, jwt) = DecodeToken(token); 
            return jwt.ValidTo; 
        }
        public string GetUsernameFromToken(string token) { 
            var (_, jwt) = DecodeToken(token); 
            return jwt.Claims.First(claim => claim.Type == ClaimTypes.Name).Value; 
        }
        public bool IsTokenExpired(string token) {
            return DateTime.Now < GetExpirationDateFromToken(token); 
        }
        public bool ValidateToken(string token, UserDetails details) { 
            return GetUsernameFromToken(token).Equals(details.Username); 
        }
    }
}
