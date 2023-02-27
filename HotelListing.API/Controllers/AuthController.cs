using HotelListing.API.Interfaces;
using HotelListing.API.Models.User;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepo _authRepo;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthRepo authRepo, ILogger<AuthController> logger)
        {
            _authRepo = authRepo;
            _logger = logger;
        }

        //POST: api/Auth/register
        [HttpPost]
        [Route("register")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Register([FromBody] UserDto userDto)
        {
            _logger.LogInformation($"Registration of {userDto.Email}");
            
            var errors = await _authRepo.Register(userDto);

            if (errors.Any()) 
            {
                foreach(var error in errors) 
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }

                return BadRequest(ModelState);
            }

            return Ok();
        }

        //POST: api/Auth/login
        [HttpPost]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Login([FromBody] UserLoginDto userLoginDto)
        {
            _logger.LogInformation($"LOGIN: {userLoginDto.Email}");

            var userAuth = await _authRepo.Login(userLoginDto);

            if (userAuth is null)
            {
                _logger.LogWarning($"UNAUTHORIZED LOGIN ATTEMPT ON {userLoginDto.Email}");
                return Unauthorized();
            }

            return Ok(userAuth);
        }

        //POST: api/Auth/refreshtoken
        [HttpPost]
        [Route("refreshtoken")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> RefreshToken([FromBody] UserAuthDto request)
        {
            var userAuth = await _authRepo.VerifyRefreshToken(request);

            if (userAuth is null)
            {
                return Unauthorized();
            }

            return Ok(userAuth);
        }
    }
}
