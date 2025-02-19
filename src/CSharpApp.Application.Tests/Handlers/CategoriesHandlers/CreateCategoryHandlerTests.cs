using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Options;
using CSharpApp.Application.Categories.Handlers;
using CSharpApp.Core.Dtos.Categories;
using CSharpApp.Application.Categories.Commands;
using CSharpApp.Core.Interfaces;
using CSharpApp.Core.Settings;

namespace CSharpApp.Application.Tests.Handlers.CategoriesHandlers {
    public class CreateCategoryHandlerTests {
        private readonly Mock<ICategoriesService> _mockCategoriesService;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly CreateCategoryHandler _handler;
        private readonly CacheSettings _cacheSettings;

        public CreateCategoryHandlerTests() {
            _mockCategoriesService = new Mock<ICategoriesService>();
            _mockCacheService = new Mock<ICacheService>();

            _cacheSettings = new CacheSettings { CategoriesKey = "categories_cache_key" };
            var mockCacheSettingsOptions = Options.Create(_cacheSettings);

            _handler = new CreateCategoryHandler(_mockCategoriesService.Object, _mockCacheService.Object, mockCacheSettingsOptions);
        }

        [Fact]
        public async Task Handle_ShouldCallCreateCategoryAndRemoveCache_WhenCalled() {
            // Arrange
            var createCategoryDto = new CategoryMutateDto { Name = "New Category", Image = "https://test.png" };
            var command = new CreateCategoryCommand(createCategoryDto);
            var createdCategory = new Category { Id = 1, Name = "New Category", Image = "https://test.png" };

            _mockCategoriesService
                .Setup(service => service.CreateCategoryAsync(createCategoryDto))
                .ReturnsAsync(createdCategory);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(createdCategory.Id, result.Id);
            Assert.Equal(createdCategory.Name, result.Name);

            _mockCategoriesService.Verify(service => service.CreateCategoryAsync(createCategoryDto), Times.Once);
            _mockCacheService.Verify(cache => cache.Remove(_cacheSettings.CategoriesKey), Times.Once);
        }
    }
}
