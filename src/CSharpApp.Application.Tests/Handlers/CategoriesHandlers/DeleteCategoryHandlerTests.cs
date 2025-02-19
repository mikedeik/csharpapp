using CSharpApp.Application.Categories.Commands;
using CSharpApp.Application.Categories.Handlers;
using CSharpApp.Core.Dtos.Categories;
using CSharpApp.Core.Interfaces;
using CSharpApp.Core.Settings;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpApp.Application.Tests.Handlers.CategoriesHandlers {
    public class DeleteCategoryHandlerTests {

        private readonly Mock<ICategoriesService> _mockCategoriesService;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly DeleteCategoryCommandHandler _handler;
        private readonly CacheSettings _cacheSettings;

        public DeleteCategoryHandlerTests() {
            _mockCategoriesService = new Mock<ICategoriesService>();
            _mockCacheService = new Mock<ICacheService>();

            _cacheSettings = new CacheSettings { CategoriesKey = "categories_cache_key" };
            var mockCacheSettingsOptions = Options.Create(_cacheSettings);

            _handler = new DeleteCategoryCommandHandler(_mockCategoriesService.Object, _mockCacheService.Object, mockCacheSettingsOptions);
        }

        [Fact]
        public async Task Handle_ShouldCallUpdateCategoryAndRemoveCache_WhenCalled() {
            // Arrange
            int categoryId = 1;
            var command = new DeleteCategoryCommand(categoryId);

            _mockCategoriesService
                .Setup(service => service.DeleteCategoryAsync(categoryId));

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockCategoriesService.Verify(service => service.DeleteCategoryAsync(categoryId), Times.Once);
            _mockCacheService.Verify(cache => cache.Remove(_cacheSettings.CategoriesKey), Times.Once);
            _mockCacheService.Verify(cache => cache.Remove($"{_cacheSettings.CategoriesKey}_{categoryId}"), Times.Once);
        }
    }
}
