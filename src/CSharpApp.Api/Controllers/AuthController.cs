using Asp.Versioning;
using CSharpApp.Core.Dtos.Auth;
using Microsoft.AspNetCore.Mvc;

namespace CSharpApp.Api.Controllers {

    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class AuthController : ControllerBase {

        private readonly IAuthenticationService _authenticationService;
        public AuthController(IAuthenticationService authenticationService) {
            _authenticationService = authenticationService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginCredentials loginCredentials) {

            return Ok( await _authenticationService.Login(loginCredentials));
        }
        
    }
}
