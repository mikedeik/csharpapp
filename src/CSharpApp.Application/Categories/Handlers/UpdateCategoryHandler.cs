using CSharpApp.Application.Categories.Commands;
using CSharpApp.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpApp.Application.Categories.Handlers {
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand> {
        private readonly ICategoriesService _categoriesService;
        private readonly ICacheService _cacheService;
        private readonly CacheSettings _cacheSettings;

        public UpdateCategoryCommandHandler(ICategoriesService categoriesService, ICacheService cacheService, IOptions<CacheSettings> options) {
            _categoriesService = categoriesService;
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
            _cacheSettings = options.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task Handle(UpdateCategoryCommand request, CancellationToken cancellationToken) {
            await _categoriesService.UpdateCategoryAsync(request.CategoryId, request.UpdatedCategory);
            _cacheService.Remove($"{_cacheSettings.CategoriesKey}_{request.CategoryId}");
            _cacheService?.Remove(_cacheSettings.CategoriesKey);
        }
    }

}
