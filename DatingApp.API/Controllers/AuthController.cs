using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.DTOS;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _repo = repo;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegister){
            
            userForRegister.Username = userForRegister.Username.ToLower();
            if(await _repo.UserExists(userForRegister.Username)){
                return BadRequest("Username already exists");
            }

            var newUser = new User{
                Username = userForRegister.Username
            };

            var createdUser = await _repo.Register(newUser, userForRegister.Password);
            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto){
            //Checking if user details are correct. 
            var userFromRepo = await _repo.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password);

            if(userFromRepo == null){
                return Unauthorized();
            }

            //Passing 2 types of claims userid and username.
            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier,userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name,userFromRepo.Username)
            };

            //To get the security key for jwt 
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            //Preparing signing credentials for JWT with key and algorith.
            var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);

            //Preparing token description
            var tokenDescriptor = new SecurityTokenDescriptor{
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            //Creating token handler which we will use to create token.
            var tokenHandler = new JwtSecurityTokenHandler();

            //Using token handler to create token using the token description.
            var token = tokenHandler.CreateToken(tokenDescriptor);

            //Returning status with JWT token.
            return Ok(new {
                token = tokenHandler.WriteToken(token)
            });
        }
        

    }
}