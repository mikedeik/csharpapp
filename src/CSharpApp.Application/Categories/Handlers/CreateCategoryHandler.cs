using CSharpApp.Application.Categories.Commands;
using CSharpApp.Core.Dtos.Categories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpApp.Application.Categories.Handlers {
    public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, Category> {
        private readonly ICategoriesService _categoriesService;
        private readonly ICacheService _cacheService;
        private readonly CacheSettings _cacheSettings;

        public CreateCategoryHandler(ICategoriesService categoriesService, ICacheService cacheService, IOptions<CacheSettings> options) {
            _categoriesService = categoriesService;
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
            _cacheSettings = options.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<Category> Handle(CreateCategoryCommand request, CancellationToken cancellationToken) {
            var category = await _categoriesService.CreateCategoryAsync(request.MutateDto);
            _cacheService.Remove(_cacheSettings.CategoriesKey);
            return category;
        }
    }
}
