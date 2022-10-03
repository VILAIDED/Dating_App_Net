using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Enities;
using DatingApp.API.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthContoller : BaseController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        public AuthContoller(DataContext dataContext,ITokenService tokenService){
            _tokenService = tokenService;
            _context = dataContext;
        }
        [HttpPost("register")]
        public IActionResult Register([FromBody] AuthUserDto authUserDto
        )
        {
            authUserDto
            .Username = authUserDto.Username.ToLower();
            if(_context.AppUsers.Any(u => u.Username == authUserDto.Username)){
                return BadRequest("User is already registerd!");
            }
            using var hmac = new HMACSHA512();
            var passwordBytes = Encoding.UTF8.GetBytes(authUserDto.Password);
            var newUser = new User{
                Username = authUserDto.Username,
                PasswordHash = hmac.ComputeHash(passwordBytes)
                ,PasswordSalt = hmac.Key
            };
            _context.AppUsers.Add(newUser);
            _context.SaveChanges();
            return Ok(newUser.Username);
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] AuthUserDto authUserDto)
        {
            authUserDto.Username = authUserDto.Username.ToLower();
            var currentUser = _context.AppUsers.FirstOrDefault(u=> u.Username == authUserDto.Username);

            if(currentUser == null){
                return Unauthorized("Username is invalid");
            }
            using var hmac = new HMACSHA512(currentUser.PasswordSalt);
            var passwordBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(authUserDto.Password));
            for(int i = 0 ; i < currentUser.PasswordHash.Length ; i++){
                if(currentUser.PasswordHash[i] != passwordBytes[i]){
                    return Unauthorized("Passowrd is invalid");
                }
            }
            var token = _tokenService.CreateToken(currentUser.Username);
            return Ok(token);
        }
        [Authorize]
        [HttpGet]
        public IActionResult Get(){
            return Ok(_context.AppUsers.ToList());
        }
     
    }
}