using CSharpApp.Application.Products.Commands;
using CSharpApp.Application.Products.Handlers;
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
    public class CreateProductHandlerTests {

        private readonly Mock<IProductsService> _mockProductsService;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly Mock<IOptions<CacheSettings>> _mockOptions;
        private readonly CacheSettings _cacheSettings;
        private readonly CreateProductHandler _handler;



        public CreateProductHandlerTests() {

            _mockProductsService = new Mock<IProductsService>();
            _mockCacheService = new Mock<ICacheService>();
            _mockOptions = new Mock<IOptions<CacheSettings>>();

            _cacheSettings = new CacheSettings {
                ProductsKey = "product_cache",
                CacheMinutesDurationCategories = 1
            };

            _mockOptions.Setup(o => o.Value).Returns(_cacheSettings);

            _handler = new CreateProductHandler(
                _mockProductsService.Object,
                _mockCacheService.Object,
                _mockOptions.Object
            );

        }

        [Fact]
        public async Task Handle_ShouldCallCreateProductAndRemoveCacheOnce_WhenCalled() {

            // Arrange
            var newProduct = new ProductCreateDto {
                CategoryId = 1,
                Title = "Title",
                Description = "Description",
                Images = ["https://test.png"],
                Price = 1,
            };

            var createdProduct = new Product {
                Id = 1,
                Title = "Title",
                Category = new Category {
                    Id = 2,
                    Name = "Name",
                    Image = "Https://test.png"
                },
                Description = "Description",
                Images = ["https://test.png"]
            };

            _mockProductsService.Setup(s => s.CreateProductAsync(newProduct)).ReturnsAsync(createdProduct);

            // Act
            var result = await _handler.Handle(new CreateProductCommand(newProduct), CancellationToken.None);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(createdProduct.Id, result.Id);
            Assert.Equal(createdProduct.Title, result.Title);

            _mockProductsService.Verify(service => service.CreateProductAsync(newProduct), Times.Once);
            _mockCacheService.Verify(cache => cache.Remove(_cacheSettings.ProductsKey), Times.Once);

        }

    }
}
