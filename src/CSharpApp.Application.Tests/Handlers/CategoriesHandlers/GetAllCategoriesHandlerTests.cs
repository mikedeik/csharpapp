using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpApp.Application.Tests.Handlers.CategoriesHandlers {
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Moq;
    using Xunit;
    using Microsoft.Extensions.Options;
    using CSharpApp.Application.Categories.Handlers;
    using CSharpApp.Application.Categories.Queries;
    using CSharpApp.Core.Dtos.Categories;
    using CSharpApp.Core.Interfaces;
    using CSharpApp.Core.Settings;

    public class GetAllCategoriesHandlerTests {
        private readonly Mock<ICategoriesService> _mockCategoriesService;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly Mock<IOptions<CacheSettings>> _mockOptions;
        private readonly CacheSettings _cacheSettings;
        private readonly GetAllCategoriesHandler _handler;

        public GetAllCategoriesHandlerTests() {
            _mockCategoriesService = new Mock<ICategoriesService>();
            _mockCacheService = new Mock<ICacheService>();
            _mockOptions = new Mock<IOptions<CacheSettings>>();

            _cacheSettings = new CacheSettings {
                CategoriesKey = "categories",
                CacheMinutesDurationCategories = 5
            };

            _mockOptions.Setup(o => o.Value).Returns(_cacheSettings);

            _handler = new GetAllCategoriesHandler(
                _mockCategoriesService.Object,
                _mockCacheService.Object,
                _mockOptions.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnCategoriesFromCache_WhenCacheHasCategories() {
            // Arrange
            var expectedCategories = new List<Category> { new Category { Id = 1, Name = "Category1", Image = "https://test.png" } };
            _mockCacheService.Setup(c => c.GetOrCreateAsync(It.IsAny<string>(), It.IsAny<Func<Task<IReadOnlyCollection<Category>>>>(), It.IsAny<TimeSpan>()))
                             .ReturnsAsync(expectedCategories);

            // Act
            var result = await _handler.Handle(new GetAllCategoriesQuery(), CancellationToken.None);

            // Assert
            Assert.Equal(expectedCategories, result);
        }

        [Fact]
        public async Task Handle_ShouldCallCategoriesService_WhenCacheMiss() {
            // Arrange
            var expectedCategories = new List<Category> { new Category { Id = 1, Name = "Category1", Image = "https://test.png" } };
            _mockCategoriesService.Setup(s => s.GetCategoriesAsync()).ReturnsAsync(expectedCategories);

            _mockCacheService.Setup(c => c.GetOrCreateAsync(It.IsAny<string>(), It.IsAny<Func<Task<IReadOnlyCollection<Category>>>>(), It.IsAny<TimeSpan>()))
              .ReturnsAsync(
                  // Simulate a cache miss by invoking the function that should fetch categories
                  await _mockCategoriesService.Object.GetCategoriesAsync()
              );

            // Act
            var result = await _handler.Handle(new GetAllCategoriesQuery(), CancellationToken.None);

            // Assert
            Assert.Equal(expectedCategories, result);
            _mockCategoriesService.Verify(s => s.GetCategoriesAsync(), Times.Once);
        }

       
    }
}
