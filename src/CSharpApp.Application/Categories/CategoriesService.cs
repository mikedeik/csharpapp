using CSharpApp.Core.Dtos.Categories;
using CSharpApp.Core.Dtos.Products;
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

        public async Task<IReadOnlyCollection<Category>> GetCategoriesAsync() {
            
            var client = _httpClientFactory.CreateClient("fakeapi");
            var response = await client.GetAsync(_restApiSettings.Categories);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var categories = JsonSerializer.Deserialize<List<Category>>(content);

            return categories.AsReadOnly();
        }

        public Task<Category> GetCategoryByIdAsync(int categoryId) {
            throw new NotImplementedException();
        }

        public Task UpdateCategoryAsync(int categoryId, CategoryMutateDto updatedCategory) {
            throw new NotImplementedException();
        }
    }
}
