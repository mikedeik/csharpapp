using CSharpApp.Core.Dtos.Auth;
using System.Text;
using System.Net;
using CSharpApp.Core.Exceptions;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Serialization;

namespace CSharpApp.Application.Authentication {
    public class AuthenticationService : IAuthenticationService {

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly RestApiSettings _restApiSettings;
        private readonly AuthenticationSettings _authenticationSettings;

        public AuthenticationService(IHttpClientFactory httpClientFactory, 
            IOptions<RestApiSettings> options,
            IOptions<AuthenticationSettings> authenticationSettings) {
            _httpClientFactory = httpClientFactory;
            _restApiSettings = options.Value;
            _authenticationSettings = authenticationSettings.Value;
        }
        public async Task<AuthResponse> Login(LoginCredentials loginCredentials) {

            var client = _httpClientFactory.CreateClient(_restApiSettings.APIName);

            
            var context = new StringContent(JsonSerializer.Serialize(loginCredentials), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(_restApiSettings.Auth, context);

            if (!response.IsSuccessStatusCode) {

                var errorResponse = await response.Content.ReadAsStringAsync();

                if(response.StatusCode == HttpStatusCode.Unauthorized) {
                    throw new UnauthorizedException("Wrong Login Credentials");
                }

                throw new BadRequestException("Authentication Failed", errorResponse);
            }


            return new AuthResponse {
                AccessToken = GenerateAccessToken(loginCredentials.Email),
                RefreshToken = GenerateRefreshToken(loginCredentials.Email),
            };

        }

        public async Task<AuthResponse> Refresh(string refreshToken) {

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(refreshToken);
            string email = jwtToken.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value ?? throw new UnauthorizedException("The refresh token is not valid");

            // A simple validation that the user exists since we are not storing/forwarding tokens of the 
            // external api will be to call the users/is-available for the stored email.
            // If it's active validation fails

            var client = _httpClientFactory.CreateClient(_restApiSettings.APIName);
            var loginCredentials = new LoginCredentials {
                Email = email,
                Password = null
            };

            var options = new JsonSerializerOptions {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            var context = new StringContent(JsonSerializer.Serialize(loginCredentials, options), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(_restApiSettings.UserAvailable, context);

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var userAvailable = JsonSerializer.Deserialize<UserAvailableDto>(responseContent);

            if (userAvailable.IsAvailable) throw new UnauthorizedException("No active user found. Please login.");

            return new AuthResponse {
                AccessToken = GenerateAccessToken(email),
                RefreshToken = GenerateRefreshToken(email),
            };

        }


        private string GenerateAccessToken(string email) {

            var claims = new List<Claim>() {
                new Claim(ClaimTypes.Name, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_authenticationSettings.AccessTokenExpirationMinutes),
                Issuer = _authenticationSettings.Issuer,
                Audience = _authenticationSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_authenticationSettings.Key)),
                    SecurityAlgorithms.HmacSha256)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);

        }

        private string GenerateRefreshToken(string email) {
            var claims = new List<Claim>() {
                new Claim(ClaimTypes.Name, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_authenticationSettings.AccessTokenExpirationMinutes),
                Issuer = _authenticationSettings.Issuer,
                Audience = _authenticationSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_authenticationSettings.Key)),
                    SecurityAlgorithms.HmacSha256)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);


        }
    }
}
