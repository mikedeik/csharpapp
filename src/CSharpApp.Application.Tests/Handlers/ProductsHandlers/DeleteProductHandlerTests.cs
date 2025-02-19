using CSharpApp.Application.Products.Commands;
using CSharpApp.Application.Products.Handlers;
using CSharpApp.Core.Interfaces;
using CSharpApp.Core.Settings;
using Microsoft.Extensions.Options;
using Moq;


namespace CSharpApp.Application.Tests.Handlers.ProductsHandlers {
    public class DeleteProductHandlerTests {

        private readonly Mock<IProductsService> _mockProductsService;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly Mock<IOptions<CacheSettings>> _mockOptions;
        private readonly CacheSettings _cacheSettings;
        private readonly DeleteProductCommandHandler _handler;

        public DeleteProductHandlerTests() {

            _mockProductsService = new Mock<IProductsService>();
            _mockCacheService = new Mock<ICacheService>();
            _mockOptions = new Mock<IOptions<CacheSettings>>();

            _cacheSettings = new CacheSettings {
                ProductsKey = "product_cache",
                CacheMinutesDurationCategories = 1
            };

            _mockOptions.Setup(o => o.Value).Returns(_cacheSettings);

            _handler = new DeleteProductCommandHandler(
                _mockProductsService.Object,
                _mockCacheService.Object,
                _mockOptions.Object
            );
        }


        [Fact]
        public async Task Handle_ShouldCallDeleteProductAndRemoveCacheOnce_WhenCalled() {

            // Arrange
            int productId = 1;
            var command = new DeleteProductCommad(productId);

            _mockProductsService.Setup(s => s.DeleteProductAsync(productId));

            //Act
            await _handler.Handle(command, CancellationToken.None);

            //Assert
            _mockProductsService.Verify(s => s.DeleteProductAsync(productId), Times.Once());
            _mockCacheService.Verify(c => c.Remove($"{_cacheSettings.ProductsKey}_{productId}"), Times.Once());
            _mockCacheService.Verify(c => c.Remove($"{_cacheSettings.ProductsKey}"), Times.Once());

        }
    }
}
