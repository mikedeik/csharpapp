using Asp.Versioning;
using CSharpApp.Application.Authentication.Commands;
using CSharpApp.Core.Dtos.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CSharpApp.Api.Controllers {

    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class AuthController : ControllerBase {

        private readonly IMediator _mediator;
        public AuthController(IMediator mediator) {
            _mediator = mediator;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginCredentials loginCredentials) {

            return Ok( await _mediator.Send(new LoginCommand(loginCredentials)));
        }

        [HttpPost("Refresh")]
        public async Task<ActionResult<AuthResponse>> Refresh([FromBody] RefreshDto refreshDto) {
            return Ok(await _mediator.Send(new RefreshCommand(refreshDto)));
        }

    }
}
