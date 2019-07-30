using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Data_Transfer_Objects;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IAuthRepository _repo;

        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _config = config;
            _repo = repo;
        }
        
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            // validate request
            // if (!ModelState.IsValid)
            //     return BadRequest(ModelState);

            userForRegisterDto.UserName = userForRegisterDto.UserName.ToLower();

            if(await _repo.UserExists(userForRegisterDto.UserName))
                return BadRequest("Username already exists");

            var  userToCreate = new User
            {
                Username = userForRegisterDto.UserName
            };

            var createdUser = await _repo.Register(userToCreate, userForRegisterDto.Password);

            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserforLoginDto userforLoginDto)
        {

            // throw new Exception("Computer says no!");
            var userFromRepo = await _repo.Login(userforLoginDto.UserName.ToLower(), userforLoginDto.Password);

            if(userFromRepo == null)
                return Unauthorized();
            
            // token has two claims: user's id and username
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Username)
            };
            // to check if it's a valid token
            // Create a key and use it as signing credentials
            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            
            // create token descriptor with expiry date of 24 hours
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };
            
            //tokenhandler allows to create tokens
            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new {
                token = tokenHandler.WriteToken(token)
            });
            }     
        
    }
}