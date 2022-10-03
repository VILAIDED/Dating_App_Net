using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DatingApp.API.Enities;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Service
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        public TokenService(IConfiguration configuration){
            _configuration = configuration;
        }
        public string CreateToken(string username)
        {
            var claims = new List<Claim>(){
                new Claim(JwtRegisteredClaimNames.NameId,username),
                new Claim(JwtRegisteredClaimNames.Email,$"{username}@dating.app")
            };
            var symmerticKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["TokenKey"]));
            var tokenDescriptor = new SecurityTokenDescriptor{
                Subject = new ClaimsIdentity(claims),
                Expires =  DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(symmerticKey,SecurityAlgorithms.HmacSha512Signature)
            };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);

        }
    }
}