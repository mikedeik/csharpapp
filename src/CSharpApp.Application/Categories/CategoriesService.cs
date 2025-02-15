using CSharpApp.Core.Dtos.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpApp.Application.Categories {
    public class CategoriesService : ICategoriesService {

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly RestApiSettings _restApiSettings;
        private readonly ILogger<CategoriesService> _logger;

        public CategoriesService(IHttpClientFactory httpClientFactory,
            IOptions<RestApiSettings> restApiSettings, 
            ILogger<CategoriesService> logger) 
            {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _restApiSettings = restApiSettings.Value ?? throw new ArgumentNullException(nameof(restApiSettings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        }

        public Task<Category> CreateCategory(CategoryMutateDto newCategory) {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<Category>> GetCategoriesAsync() {
            throw new NotImplementedException();
        }

        public Task<Category> GetCategoryByIdAsync(int categoryId) {
            throw new NotImplementedException();
        }

        public Task UpdateCategoryAsync(int categoryId, CategoryMutateDto updatedCategory) {
            throw new NotImplementedException();
        }
    }
}
