using CSharpApp.Application.Categories.Commands;
using CSharpApp.Core.Interfaces;
using CSharpApp.Core.Settings;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpApp.Application.Categories.Handlers {
    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand> {

        private readonly ICategoriesService _categoriesService;
        private readonly ICacheService _cacheService;
        private readonly CacheSettings _cacheSettings;

        public DeleteCategoryCommandHandler(ICategoriesService categoriesService, ICacheService cacheService, IOptions<CacheSettings> options) {
            _categoriesService = categoriesService;
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
            _cacheSettings = options.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken) {

            await _categoriesService.DeleteCategoryAsync(request.categoryId);
            _cacheService?.Remove($"{_cacheSettings.CategoriesKey}_{request.categoryId}");
            _cacheService?.Remove(_cacheSettings.CategoriesKey);
            
        }
    }
}
