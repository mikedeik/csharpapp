using CSharpApp.Application.Products.Handlers;
using CSharpApp.Application.Products.Queries;
using CSharpApp.Core.Dtos.Categories;
using CSharpApp.Core.Dtos.Products;
using CSharpApp.Core.Interfaces;
using CSharpApp.Core.Settings;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpApp.Application.Tests.Handlers.ProductsHandlers {
    public class GetProductByIdHandlerTests {

        private readonly Mock<IProductsService> _mockProductsService;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly Mock<IOptions<CacheSettings>> _mockOptions;
        private readonly CacheSettings _cacheSettings;
        private readonly GetProductByIdHandler _handler;

        public GetProductByIdHandlerTests() {

            _mockProductsService = new Mock<IProductsService>();
            _mockCacheService = new Mock<ICacheService>();
            _mockOptions = new Mock<IOptions<CacheSettings>>();

            _cacheSettings = new CacheSettings {
                ProductsKey = "product_cache",
                CacheMinutesDurationCategories = 1
            };

            _mockOptions.Setup(o => o.Value).Returns(_cacheSettings);

            _handler = new GetProductByIdHandler(
                _mockProductsService.Object,
                _mockCacheService.Object,
                _mockOptions.Object
            );
        }


        [Fact]
        public async Task Handle_ShoudReturnProductFromCache_WhenCacheHasProductWithSpecificId() {

            // Arrange
            int productId = 1;
            var cachedProduct = new Product {
                Id = 1,
                Title = "Title",
                Category = new Category {
                    Id = 2,
                    Name = "Name",
                    Image = "Https://test.png"
                },
                Description = "Description",
                Images = ["https://test.image.png"]
            };

            _mockCacheService
                .Setup(c => c.GetOrCreateAsync(
                    $"{_cacheSettings.ProductsKey}_{productId}",
                    It.IsAny<Func<Task<Product>>>(),
                    It.IsAny<TimeSpan>()
                ))
                .ReturnsAsync(cachedProduct);

            // Act
            var result = await _handler.Handle(new GetProductByIdQuery(productId), CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(cachedProduct, result);

        }

        [Fact]
        public async Task Handle_ShouldCallServiceMethod_WhenCacheMisses() {

            // Arrange
            int productId = 1;
            var expectedProduct = new Product {

                Id = 1,
                Title = "Title",
                Category = new Category {
                    Id = 2,
                    Name = "Name",
                    Image = "Https://test.png"
                },
                Description = "Description",
                Images = ["https://test.image.png"]
            };

            _mockProductsService.Setup(s => s.GetProductByIdAsync(productId)).ReturnsAsync(expectedProduct);

            _mockCacheService.Setup(c => c.GetOrCreateAsync(
                It.IsAny<string>(),
                It.IsAny<Func<Task<Product>>>(),
                It.IsAny<TimeSpan>()
                )).ReturnsAsync(
                    await _mockProductsService.Object.GetProductByIdAsync(productId)
                );

            // Act
            var results = await _handler.Handle(new GetProductByIdQuery(productId), CancellationToken.None);

            //Assert
            Assert.Equal(expectedProduct, results);
            _mockProductsService.Verify(s => s.GetProductByIdAsync(productId), Times.Once);


        }



    }
}
