using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpApp.Application.Tests.Handlers.CategoriesHandlers {
    using System;
    using Moq;
    using System.Threading.Tasks;
    using System.Threading;
    using Microsoft.Extensions.Options;
    using Xunit;
    using CSharpApp.Application.Categories.Handlers;
    using CSharpApp.Application.Categories.Queries;
    using CSharpApp.Core.Dtos.Categories;
    using CSharpApp.Core.Interfaces;
    using CSharpApp.Core.Settings;

    public class GetCategoryByIdHandlerTests {
        private readonly Mock<ICategoriesService> _mockCategoriesService;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly Mock<IOptions<CacheSettings>> _mockOptions;
        private readonly CacheSettings _cacheSettings;
        private readonly GetCategoryByIdHandler _handler;

        public GetCategoryByIdHandlerTests() {
            _mockCategoriesService = new Mock<ICategoriesService>();
            _mockCacheService = new Mock<ICacheService>();
            _mockOptions = new Mock<IOptions<CacheSettings>>();

            _cacheSettings = new CacheSettings {
                CategoriesKey = "categories",
                CacheMinutesDurationCategories = 5
            };

            _mockOptions.Setup(o => o.Value).Returns(_cacheSettings);

            _handler = new GetCategoryByIdHandler(
                _mockCategoriesService.Object,
                _mockCacheService.Object,
                _mockOptions.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnCategoryFromCache_WhenCacheHasCategory() {
            // Arrange
            var categoryId = 1;
            var cachedCategory = new Category { Id = categoryId, Name = "Cached Category" };
            var request = new GetCategoryByIdQuery(categoryId);

            _mockCacheService
                .Setup(c => c.GetOrCreateAsync(
                    $"{_cacheSettings.CategoriesKey}_{categoryId}",
                    It.IsAny<Func<Task<Category>>>(),
                    It.IsAny<TimeSpan>()
                ))
                .ReturnsAsync(cachedCategory);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(cachedCategory.Id, result.Id);
            Assert.Equal(cachedCategory.Name, result.Name);
            _mockCategoriesService.Verify(s => s.GetCategoryByIdAsync(categoryId), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldCallCategoriesService_WhenCacheMiss() {
            // Arrange
            var categoryId = 1;
            var fetchedCategory = new Category { Id = categoryId, Name = "Fetched Category" };
            var request = new GetCategoryByIdQuery (categoryId);


            _mockCategoriesService
                .Setup(s => s.GetCategoryByIdAsync(categoryId))
                .ReturnsAsync(fetchedCategory);

            _mockCacheService
                .Setup(c => c.GetOrCreateAsync(
                    $"{_cacheSettings.CategoriesKey}_{categoryId}",
                    It.IsAny<Func<Task<Category>>>(),
                    It.IsAny<TimeSpan>()
                ))
                .ReturnsAsync(
                    await _mockCategoriesService.Object.GetCategoryByIdAsync(categoryId)
                );

            

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(fetchedCategory.Id, result.Id);
            Assert.Equal(fetchedCategory.Name, result.Name);
            _mockCategoriesService.Verify(s => s.GetCategoryByIdAsync(categoryId), Times.Once);
        }
    }

}
