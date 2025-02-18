using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using CSharpApp.Core.Dtos.Products;
using CSharpApp.Core.Exceptions;
using CSharpApp.Application.Products;
using CSharpApp.Core.Settings;
using CSharpApp.Core.Dtos.Categories;

namespace CSharpApp.Application.Tests.Products {
    public class ProductsServiceTests {
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
        private readonly Mock<IOptions<RestApiSettings>> _restApiSettingsMock;
        private readonly ProductsService _productsService;

        public ProductsServiceTests() {
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _restApiSettingsMock = new Mock<IOptions<RestApiSettings>>();

            var restApiSettings = new RestApiSettings {
                APIName = "TestAPI",
                Products = "api/products"
            };
            _restApiSettingsMock.Setup(x => x.Value).Returns(restApiSettings);

            _productsService = new ProductsService(
                _restApiSettingsMock.Object,
                _httpClientFactoryMock.Object
            );
        }

        [Fact]
        public async Task CreateProductAsync_ShouldReturnProduct_WhenRequestIsSuccessful() {
            // Arrange
            var newProduct = new ProductCreateDto { 
                Title = "Test Product", 
                Price = 50, 
                Description = "A trendy and comfortable pair of sneakers.",
                Images = new List<string> {
                        "https://demo.com/images/green-sneakers.jpg"
                },
                CategoryId = 10
            };
            var expectedProduct = new Product {
                Id = 1,
                Title = "Demo Product - Green Sneakers",
                Price = 50,
                Description = "A trendy and comfortable pair of sneakers.",
                Images = new List<string> {
                        "[\"https://demo.com/images/green-sneakers.jpg\"]"
                    },
                CreationAt = DateTime.Parse("2024-10-01T12:00:00.000Z"),
                UpdatedAt = DateTime.Parse("2024-10-02T14:30:00.000Z"),
                Category = new Category {
                    Id = 10,
                    Name = "Footwear",
                    Image = "https://demo.com/images/footwear-category.jpg",
                    CreationAt = DateTime.Parse("2024-09-25T09:30:00.000Z"),
                    UpdatedAt = DateTime.Parse("2024-09-26T11:15:00.000Z")
                }
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
                    Content = new StringContent(JsonSerializer.Serialize(expectedProduct))
                });

            var httpClient = new HttpClient(httpClientMock.Object) {
                BaseAddress = new Uri("http://localhost")
            };
            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Act
            var result = await _productsService.CreateProductAsync(newProduct);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedProduct.Id, result.Id);
            Assert.Equal(expectedProduct.Title, result.Title);
            Assert.Equal(expectedProduct.Category.Id, result.Category.Id);
            Assert.Equal("https://demo.com/images/green-sneakers.jpg", result.Images[0]);

        }

        [Fact]
        public async Task CreateProductAsync_ShouldThrowBadRequestException_WhenRequestFails() {
            // Arrange
            var newProduct = new ProductCreateDto {
                Title = "",
                Price = 50,
                Description = "A trendy and comfortable pair of sneakers.",
                Images = new List<string> {
                        "https://demo.com/images/green-sneakers.jpg"
                },
                CategoryId = 10
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
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent("Validation error"),
                });

            var httpClient = new HttpClient(httpClientMock.Object) {
                BaseAddress = new Uri("http://localhost")
            };
            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _productsService.CreateProductAsync(newProduct));
        }

        [Fact]
        public async Task GetProductsAsync_ShouldReturnListOfProductsAndConvertMalformedUrlList_WhenRequestIsSuccessful() {
            // Arrange
            var demoProducts = new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Title = "Demo Product - Green Sneakers",
                    Price = 50,
                    Description = "A trendy and comfortable pair of sneakers.",
                    Images = new List<string> {
                        "[\"https://demo.com/images/green-sneakers.jpg\"]"
                    },
                    CreationAt = DateTime.Parse("2024-10-01T12:00:00.000Z"),
                    UpdatedAt = DateTime.Parse("2024-10-02T14:30:00.000Z"),
                    Category = new Category
                    {
                        Id = 10,
                        Name = "Footwear",
                        Image = "https://demo.com/images/footwear-category.jpg",
                        CreationAt = DateTime.Parse("2024-09-25T09:30:00.000Z"),
                        UpdatedAt = DateTime.Parse("2024-09-26T11:15:00.000Z")
                    }
                },
                new Product
                {
                    Id = 2,
                    Title = "Demo Product - Wireless Headphones",
                    Price = 150,
                    Description = "High-quality sound with noise cancellation.",
                    Images = new List<string> {
                        "[\"https://demo.com/images/wireless-headphones.jpg\"]"
                    },
                    CreationAt = DateTime.Parse("2024-11-15T16:45:00.000Z"),
                    UpdatedAt = DateTime.Parse("2024-11-16T09:20:00.000Z"),
                    Category = new Category
                    {
                        Id = 11,
                        Name = "Electronics",
                        Image = "https://demo.com/images/electronics-category.jpg",
                        CreationAt = DateTime.Parse("2024-11-10T07:30:00.000Z"),
                        UpdatedAt = DateTime.Parse("2024-11-12T13:00:00.000Z")
                    }
                }
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
                    Content = new StringContent(JsonSerializer.Serialize(demoProducts)),
                });

            var httpClient = new HttpClient(httpClientMock.Object) {
                BaseAddress = new Uri("http://localhost")
            };
            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Act
            var result = await _productsService.GetProductsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());

            var product1 = result.First();
            Assert.Equal(1, product1.Id);
            Assert.Equal("Demo Product - Green Sneakers", product1.Title);
            Assert.Equal(50, product1.Price);
            Assert.Equal("A trendy and comfortable pair of sneakers.", product1.Description);
            Assert.Single(product1.Images);
            Assert.Equal("https://demo.com/images/green-sneakers.jpg", product1.Images[0]);
            Assert.Equal(DateTime.Parse("2024-10-01T12:00:00.000Z"), product1.CreationAt);
            Assert.Equal(DateTime.Parse("2024-10-02T14:30:00.000Z"), product1.UpdatedAt);
            Assert.NotNull(product1.Category);
            Assert.Equal(10, product1.Category.Id);
            Assert.Equal("Footwear", product1.Category.Name);
            Assert.Equal("https://demo.com/images/footwear-category.jpg", product1.Category.Image);
            Assert.Equal(DateTime.Parse("2024-09-25T09:30:00.000Z"), product1.Category.CreationAt);
            Assert.Equal(DateTime.Parse("2024-09-26T11:15:00.000Z"), product1.Category.UpdatedAt);

            var product2 = result.Last();
            Assert.Equal(2, product2.Id);
            Assert.Equal("Demo Product - Wireless Headphones", product2.Title);
            Assert.Equal(150, product2.Price);
            Assert.Equal("High-quality sound with noise cancellation.", product2.Description);
            Assert.Single(product2.Images);
            Assert.Equal("https://demo.com/images/wireless-headphones.jpg", product2.Images[0]);
            Assert.Equal(DateTime.Parse("2024-11-15T16:45:00.000Z"), product2.CreationAt);
            Assert.Equal(DateTime.Parse("2024-11-16T09:20:00.000Z"), product2.UpdatedAt);
            Assert.NotNull(product2.Category);
            Assert.Equal(11, product2.Category.Id);
            Assert.Equal("Electronics", product2.Category.Name);
            Assert.Equal("https://demo.com/images/electronics-category.jpg", product2.Category.Image);
            Assert.Equal(DateTime.Parse("2024-11-10T07:30:00.000Z"), product2.Category.CreationAt);
            Assert.Equal(DateTime.Parse("2024-11-12T13:00:00.000Z"), product2.Category.UpdatedAt);
        }

        [Fact]
        public async Task GetProductAsync_ShouldReturnSingleProduct_WhenRequestIsSuccessful() {
            // Arrange
            var demoProduct = new Product {
                Id = 1,
                Title = "Demo Product - Green Sneakers",
                Price = 50,
                Description = "A trendy and comfortable pair of sneakers.",
                Images = new List<string> {
            "[\"https://demo.com/images/green-sneakers.jpg\"]"
        },
                CreationAt = DateTime.Parse("2024-10-01T12:00:00.000Z"),
                UpdatedAt = DateTime.Parse("2024-10-02T14:30:00.000Z"),
                Category = new Category {
                    Id = 10,
                    Name = "Footwear",
                    Image = "https://demo.com/images/footwear-category.jpg",
                    CreationAt = DateTime.Parse("2024-09-25T09:30:00.000Z"),
                    UpdatedAt = DateTime.Parse("2024-09-26T11:15:00.000Z")
                }
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
                    Content = new StringContent(JsonSerializer.Serialize(demoProduct)),
                });

            var httpClient = new HttpClient(httpClientMock.Object) {
                BaseAddress = new Uri("http://localhost")
            };
            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Act
            var result = await _productsService.GetProductByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Demo Product - Green Sneakers", result.Title);
            Assert.Equal(50, result.Price);
            Assert.Equal("A trendy and comfortable pair of sneakers.", result.Description);
            Assert.Single(result.Images);
            Assert.Equal("https://demo.com/images/green-sneakers.jpg", result.Images[0]);
            Assert.Equal(DateTime.Parse("2024-10-01T12:00:00.000Z"), result.CreationAt);
            Assert.Equal(DateTime.Parse("2024-10-02T14:30:00.000Z"), result.UpdatedAt);
            Assert.NotNull(result.Category);
            Assert.Equal(10, result.Category.Id);
            Assert.Equal("Footwear", result.Category.Name);
            Assert.Equal("https://demo.com/images/footwear-category.jpg", result.Category.Image);
            Assert.Equal(DateTime.Parse("2024-09-25T09:30:00.000Z"), result.Category.CreationAt);
            Assert.Equal(DateTime.Parse("2024-09-26T11:15:00.000Z"), result.Category.UpdatedAt);
        }

        [Fact]
        public async Task GetProductAsync_ShoulThrowNotFoundException_WhenProductDoesNotExist() {

            int productId = 9999;
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

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _productsService.GetProductByIdAsync(productId));

        }


        [Fact]
        public async Task UpdateProductAsync_ShouldCompleteSuccessfully_WhenProductExists() {
            // Arrange
            var productId = 1;
            var updatedProduct = new ProductUpdateDto { Title = "Updated Product" };
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
            await _productsService.UpdateProductAsync(productId, updatedProduct);

            // Assert
            // No exception means success
        }

        [Fact]
        public async Task UpdateProductAsync_ShouldThrowNotFoundException_WhenProductDoesNotExist() {
            // Arrange
            var productId = 1;
            var updatedProduct = new ProductUpdateDto { Title = "Updated Product" };
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
            await Assert.ThrowsAsync<NotFoundException>(() => _productsService.UpdateProductAsync(productId, updatedProduct));
        }


        [Fact]
        public async Task UpdateProductAsync_ShouldThrowBadRequest_WhenTryingToUpdateEmptyImagesArray() {
            // Arrange
            var productId = 1;
            var updatedProduct = new ProductUpdateDto { Images = [] };
            var httpClientMock = new Mock<HttpMessageHandler>();
            httpClientMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage {
                    StatusCode = HttpStatusCode.BadRequest
                });

            var httpClient = new HttpClient(httpClientMock.Object) {
                BaseAddress = new Uri("http://localhost")
            };
            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _productsService.UpdateProductAsync(productId, updatedProduct));
        }
    }
}
