using Xunit;
using Moq;
using System.Net;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.Extensions.Options;
using CSharpApp.Core.Dtos.Auth;
using CSharpApp.Core.Exceptions;
using CSharpApp.Application.Authentication;
using CSharpApp.Core.Settings;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


namespace CSharpApp.Application.Tests.Services {
    public class AuthenticationServiceTests {
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
        private readonly AuthenticationService _authService;
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly RestApiSettings _restApiSettings;
        private readonly AuthenticationSettings _authSettings;

        public AuthenticationServiceTests() {
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();

            _restApiSettings = new RestApiSettings {
                APIName = "TestApi",
                Auth = "https://mockapi.com/auth",
                UserAvailable = "https://mockapi.com/user/is-available"
            };

            _authSettings = new AuthenticationSettings {
                Key = "supersecretkey1234567890needmorebytesforthesecretkey",
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                AccessTokenExpirationMinutes = 60
            };

            var restApiOptions = Options.Create(_restApiSettings);
            var authOptions = Options.Create(_authSettings);

            _authService = new AuthenticationService(_httpClientFactoryMock.Object, restApiOptions, authOptions);
        }

        [Fact]
        public async Task Login_ShouldReturnTokens_WhenCredentialsAreValid() {
            // Arrange
            var loginCredentials = new LoginCredentials { Email = "john@mail.com", Password = "changeme" };
            var httpResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.OK };

            var httpClientMock = new HttpClient(new MockHttpMessageHandler(httpResponse));
            _httpClientFactoryMock.Setup(f => f.CreateClient(_restApiSettings.APIName)).Returns(httpClientMock);

            // Act
            var result = await _authService.Login(loginCredentials);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<AuthResponse>(result);
            Assert.False(string.IsNullOrEmpty(result.AccessToken));
            Assert.False(string.IsNullOrEmpty(result.RefreshToken));
        }

        [Fact]
        public async Task Login_ShouldThrowUnauthorizedException_WhenCredentialsAreInvalid() {
            // Arrange
            var loginCredentials = new LoginCredentials { Email = "wrong@mail.com", Password = "wrongpassword" };
            var httpResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.Unauthorized };

            var httpClientMock = new HttpClient(new MockHttpMessageHandler(httpResponse));
            _httpClientFactoryMock.Setup(f => f.CreateClient(_restApiSettings.APIName)).Returns(httpClientMock);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(() => _authService.Login(loginCredentials));
        }

        [Fact]
        public async Task Refresh_ShouldReturnNewTokens_WhenRefreshTokenIsValid() {
            // Arrange
            var validRefreshToken = GenerateTestRefreshToken("john@mail.com");

            var mockResponse = new AuthResponse { AccessToken = "newAccessToken", RefreshToken = "newRefreshToken" };
            var httpResponse = new HttpResponseMessage {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(new UserAvailableDto { IsAvailable = false }), Encoding.UTF8, "application/json")
            };

            var httpClientMock = new HttpClient(new MockHttpMessageHandler(httpResponse));
            _httpClientFactoryMock.Setup(f => f.CreateClient(_restApiSettings.APIName)).Returns(httpClientMock);

            // Act
            var result = await _authService.Refresh(validRefreshToken);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<AuthResponse>(result);
            Assert.False(string.IsNullOrEmpty(result.AccessToken));
            Assert.False(string.IsNullOrEmpty(result.RefreshToken));

        }

        [Fact]
        public async Task Refresh_ShouldThrowUnauthorizedException_WhenUserIsNotActive() {
            // Arrange
            var refreshToken = GenerateTestRefreshToken("inactive@mail.com");
            var httpResponse = new HttpResponseMessage {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(new UserAvailableDto { IsAvailable = true }), Encoding.UTF8, "application/json")
            };

            var httpClientMock = new HttpClient(new MockHttpMessageHandler(httpResponse));
            _httpClientFactoryMock.Setup(f => f.CreateClient(_restApiSettings.APIName)).Returns(httpClientMock);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(() => _authService.Refresh(refreshToken));
        }

        [Fact]
        public async Task Refresh_ShouldThrowBadRequestException_WhenInputRefreshTokenIsNotValid() {
            // Arrange
            var refreshToken = "NotAValidToken";
            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _authService.Refresh(refreshToken));
        }


        // Used to generate refresh tokens for testing
        private string GenerateTestRefreshToken(string email) {
            var claims = new List<Claim>
            {
            new Claim(ClaimTypes.Name, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(15),
                Issuer = _authSettings.Issuer,
                Audience = _authSettings.Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authSettings.Key)),
                    SecurityAlgorithms.HmacSha256)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }

    // Mock HttpMessageHandler for HttpClient
    public class MockHttpMessageHandler : HttpMessageHandler {
        private readonly HttpResponseMessage _response;

        public MockHttpMessageHandler(HttpResponseMessage response) {
            _response = response;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            return await Task.FromResult(_response);
        }
    }
}
