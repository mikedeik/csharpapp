using CSharpApp.Application.Categories.Queries;
using CSharpApp.Application.Products.Handlers;
using CSharpApp.Application.Products.Queries;
using CSharpApp.Core.Dtos.Categories;
using CSharpApp.Core.Dtos.Products;
using CSharpApp.Core.Interfaces;
using CSharpApp.Core.Settings;
using Microsoft.Extensions.Options;
using Moq;


namespace CSharpApp.Application.Tests.Handlers.ProductsHandlers {
    public class GetAllProductsHandlerTests {

        private readonly Mock<IProductsService> _mockProductsService;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly Mock<IOptions<CacheSettings>> _mockOptions;
        private readonly CacheSettings _cacheSettings;
        private readonly GetAllProductsHandler _handler;

        public GetAllProductsHandlerTests() {

            _mockProductsService = new Mock<IProductsService>();
            _mockCacheService = new Mock<ICacheService>();
            _mockOptions = new Mock<IOptions<CacheSettings>>();

            _cacheSettings = new CacheSettings {
                ProductsKey = "product_cache",
                CacheMinutesDurationCategories = 1
            };

            _mockOptions.Setup(o => o.Value).Returns(_cacheSettings);

            _handler = new GetAllProductsHandler(
                _mockProductsService.Object,
                _mockCacheService.Object,
                _mockOptions.Object
            );
        }

        [Fact]
        public async Task Handle_ShoudReturnProductsFromCache_WhenCacheHasProducts() {

            // Arrange

            var cachedProducts = new List<Product> {
                new Product {
                    Id = 1,
                    Title = "Title",
                    Category = new Category {
                        Id = 2,
                        Name = "Name",
                        Image = "Https://test.png"
                    },
                    Description = "Description",
                    Images = ["https://test.image.png"]
                }
            };

            _mockCacheService
                .Setup(c => c.GetOrCreateAsync(
                    _cacheSettings.ProductsKey,
                    It.IsAny<Func<Task<IReadOnlyCollection<Product>>>>(),
                    It.IsAny<TimeSpan>()
                ))
                .ReturnsAsync(cachedProducts);

            // Act
            var result = await _handler.Handle(new GetAllProductsQuery(), CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(cachedProducts, result);

        }

        [Fact]
        public async Task Handle_ShouldCallServiceMethod_WhenCacheMisses() {

            // Arrange
            var expectedProducts = new List<Product> {
                new() {
                    Id = 1,
                    Title = "Title",
                    Category = new Category {
                        Id = 2,
                        Name = "Name",
                        Image = "Https://test.png"
                    },
                    Description = "Description",
                    Images = ["https://test.image.png"]
                }
            };

            _mockProductsService.Setup(s => s.GetProductsAsync()).ReturnsAsync(expectedProducts);

            _mockCacheService.Setup(c => c.GetOrCreateAsync(
                It.IsAny<string>(),
                It.IsAny<Func<Task<IReadOnlyCollection<Product>>>>(),
                It.IsAny<TimeSpan>()
                )).ReturnsAsync(
                    await _mockProductsService.Object.GetProductsAsync()
                );

            // Act
            var results = await _handler.Handle(new GetAllProductsQuery(), CancellationToken.None);

            //Assert
            Assert.Equal(expectedProducts, results);
            _mockProductsService.Verify(s => s.GetProductsAsync(), Times.Once);


        }
    }
}
