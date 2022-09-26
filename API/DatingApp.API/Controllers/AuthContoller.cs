using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Enities;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthContoller : BaseController
    {
        private readonly DataContext _context;
        public AuthContoller(DataContext dataContext){
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
            var passwordBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(authUserDto.Password));
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
        public void Login([FromBody] string value)
        {
        }

     
    }
}