using CSharpApp.Core.Dtos.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using CSharpApp.Core.Exceptions;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

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
                AccessToken = GenerateAccessToken(loginCredentials),
                RefreshToken = GenerateRefreshToken(loginCredentials.Email),
            };

        }

        public Task<AuthResponse> Refresh(string refreshToken) {
            throw new NotImplementedException();
        }


        private string GenerateAccessToken(LoginCredentials loginCredentials) {

            var claims = new List<Claim>() {
                new Claim(ClaimTypes.Name, loginCredentials.Email),
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
