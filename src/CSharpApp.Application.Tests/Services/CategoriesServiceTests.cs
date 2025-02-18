using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using CSharpApp.Core.Dtos.Categories;
using CSharpApp.Core.Exceptions;
using CSharpApp.Application.Categories;
using CSharpApp.Core.Settings;
using Moq.Protected;

namespace CSharpApp.Application.Tests.Categories {
    public class CategoriesServiceTests {
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
        private readonly Mock<IOptions<RestApiSettings>> _restApiSettingsMock;
        private readonly CategoriesService _categoriesService;

        public CategoriesServiceTests() {
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _restApiSettingsMock = new Mock<IOptions<RestApiSettings>>();

            var restApiSettings = new RestApiSettings {
                APIName = "TestAPI",
                Categories = "api/categories"
            };
            _restApiSettingsMock.Setup(x => x.Value).Returns(restApiSettings);

            _categoriesService = new CategoriesService(
                _httpClientFactoryMock.Object,
                _restApiSettingsMock.Object
            );
        }

        [Fact]
        public async Task CreateCategoryAsync_ShouldReturnCategory_WhenRequestIsSuccessful() {
            // Arrange
            var newCategory = new CategoryMutateDto { Name = "Test Category" };
            var expectedCategory = new Category { Id = 1, Name = "Test Category" };
            var httpClientMock = new Mock<HttpMessageHandler>();
            httpClientMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(expectedCategory))
                });

            var httpClient = new HttpClient(httpClientMock.Object) {
                BaseAddress = new Uri("http://localhost") 
            };
            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Act
            var result = await _categoriesService.CreateCategoryAsync(newCategory);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedCategory.Id, result.Id);
            Assert.Equal(expectedCategory.Name, result.Name);
        }

        [Fact]
        public async Task CreateCategoryAsync_ShouldThrowBadRequestException_WhenRequestFails() {
            // Arrange
            var newCategory = new CategoryMutateDto { Name = "Test Category" };
            var httpClientMock = new Mock<HttpMessageHandler>();
            httpClientMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent("Error message"),
                });

            var httpClient = new HttpClient(httpClientMock.Object) {
                BaseAddress = new Uri("http://localhost") 
            };
            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _categoriesService.CreateCategoryAsync(newCategory));
        }

        [Fact]
        public async Task GetCategoriesAsync_ShouldReturnListOfCategories_WhenRequestIsSuccessful() {
            // Arrange
            var expectedCategories = new List<Category>
            {
                new Category { Id = 1, Name = "Category 1" },
                new Category { Id = 2, Name = "Category 2" }
            };

            var httpClientMock = new Mock<HttpMessageHandler>();
            httpClientMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(expectedCategories)),
                });

            var httpClient = new HttpClient(httpClientMock.Object) {
                BaseAddress = new Uri("http://localhost")
            };
            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Act
            var result = await _categoriesService.GetCategoriesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetCategoryByIdAsync_ShouldReturnCategory_WhenCategoryExists() {
            // Arrange
            var categoryId = 1;
            var expectedCategory = new Category { Id = categoryId, Name = "Test Category" };

            var httpClientMock = new Mock<HttpMessageHandler>();
            httpClientMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(expectedCategory)),
                });
            var httpClient = new HttpClient(httpClientMock.Object) {
                BaseAddress = new Uri("http://localhost") 
            };
            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Act
            var result = await _categoriesService.GetCategoryByIdAsync(categoryId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedCategory.Id, result.Id);
            Assert.Equal(expectedCategory.Name, result.Name);
        }

        [Fact]
        public async Task GetCategoryByIdAsync_ShouldThrowNotFoundException_WhenCategoryDoesNotExist() {
            // Arrange
            var categoryId = 1;
            var httpClientMock = new Mock<HttpMessageHandler>();
            httpClientMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent("EntityNotFoundError"),
                });

            var httpClient = new HttpClient(httpClientMock.Object) {
                BaseAddress = new Uri("http://localhost") 
            };
            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _categoriesService.GetCategoryByIdAsync(categoryId));
        }

        [Fact]
        public async Task UpdateCategoryAsync_ShouldCompleteSuccessfully_WhenCategoryExists() {
            // Arrange
            var categoryId = 1;
            var updatedCategory = new CategoryMutateDto { Name = "Updated Category" };

            var httpClientMock = new Mock<HttpMessageHandler>();
            httpClientMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage {
                    StatusCode = HttpStatusCode.OK,
                });

            var httpClient = new HttpClient(httpClientMock.Object) {
                BaseAddress = new Uri("http://localhost") 
            };
            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Act
            await _categoriesService.UpdateCategoryAsync(categoryId, updatedCategory);

            // Assert
            // No exception means success
        }

        [Fact]
        public async Task UpdateCategoryAsync_ShouldThrowNotFoundException_WhenCategoryDoesNotExist() {
            // Arrange
            var categoryId = 1;
            var updatedCategory = new CategoryMutateDto { Name = "Updated Category" };

            var httpClientMock = new Mock<HttpMessageHandler>();
            httpClientMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent("EntityNotFoundError"),
                });

            var httpClient = new HttpClient(httpClientMock.Object) {
                BaseAddress = new Uri("http://localhost") 
            };
            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _categoriesService.UpdateCategoryAsync(categoryId, updatedCategory));
        }
    }
}