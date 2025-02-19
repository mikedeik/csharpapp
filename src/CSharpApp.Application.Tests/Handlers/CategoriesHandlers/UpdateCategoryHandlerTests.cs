using CSharpApp.Application.Categories.Commands;
using CSharpApp.Application.Categories.Handlers;
using CSharpApp.Core.Dtos.Categories;
using CSharpApp.Core.Interfaces;
using CSharpApp.Core.Settings;
using Microsoft.Extensions.Options;
using Moq;


namespace CSharpApp.Application.Tests.Handlers.CategoriesHandlers {
    public class UpdateCategoryHandlerTests {
        private readonly Mock<ICategoriesService> _mockCategoriesService;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly UpdateCategoryCommandHandler _handler;
        private readonly CacheSettings _cacheSettings;

        public UpdateCategoryHandlerTests() {
            _mockCategoriesService = new Mock<ICategoriesService>();
            _mockCacheService = new Mock<ICacheService>();

            _cacheSettings = new CacheSettings { CategoriesKey = "categories_cache_key" };
            var mockCacheSettingsOptions = Options.Create(_cacheSettings);

            _handler = new UpdateCategoryCommandHandler(_mockCategoriesService.Object, _mockCacheService.Object, mockCacheSettingsOptions);
        }

        [Fact]
        public async Task Handle_ShouldCallUpdateCategoryAndRemoveCache_WhenCalled() {
            // Arrange
            int categoryId = 1;
            var updatedCategory = new CategoryMutateDto { Name = "Updated Category", Image = "https://test.png" };
            var command = new UpdateCategoryCommand(categoryId, updatedCategory);

            _mockCategoriesService
                .Setup(service => service.UpdateCategoryAsync(categoryId, updatedCategory));

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockCategoriesService.Verify(service => service.UpdateCategoryAsync(categoryId, updatedCategory), Times.Once);
            _mockCacheService.Verify(cache => cache.Remove(_cacheSettings.CategoriesKey), Times.Once);
            _mockCacheService.Verify(cache => cache.Remove($"{_cacheSettings.CategoriesKey}_{categoryId}"), Times.Once);
        }
    }
}
