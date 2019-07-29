using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Data_Transfer_Objects;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;

        public AuthController(IAuthRepository repo)
        {
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
                return BadRequest("Username already existes");

            var  userToCreate = new User
            {
                Username = userForRegisterDto.UserName
            };

            var createdUser = await _repo.Register(userToCreate, userForRegisterDto.Password);

            return StatusCode(201);
        }
    }
}