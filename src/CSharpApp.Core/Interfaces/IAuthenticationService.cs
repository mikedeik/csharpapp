using CSharpApp.Core.Dtos.Auth;

namespace CSharpApp.Core.Interfaces {
    public interface IAuthenticationService {

        public Task<AuthResponse> Login(LoginCredentials loginCredentials);
        public Task<AuthResponse> Refresh(string refreshToken);

    }
}
