using CSharpApp.Application.Authentication.Commands;
using CSharpApp.Core.Dtos.Auth;
using MediatR;


namespace CSharpApp.Application.Authentication.Handlers {
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse> {
        private readonly IAuthenticationService _authenticationService;

        public LoginCommandHandler(IAuthenticationService authenticationService) {
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        }

        public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken token) {

            return await _authenticationService.Login(request.LoginCredentials);
        }
    }
}
