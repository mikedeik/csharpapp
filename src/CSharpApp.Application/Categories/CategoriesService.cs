using System.Net;
using System.Text;
using CSharpApp.Core.Dtos.Categories;
using CSharpApp.Core.Exceptions;


namespace CSharpApp.Application.Categories {
    public class CategoriesService : ICategoriesService {

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly RestApiSettings _restApiSettings;

        public CategoriesService(IHttpClientFactory httpClientFactory,
            IOptions<RestApiSettings> restApiSettings) 
            {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _restApiSettings = restApiSettings.Value ?? throw new ArgumentNullException(nameof(restApiSettings));

        }

        public async Task<Category> CreateCategoryAsync(CategoryMutateDto newCategory) {

            var client = _httpClientFactory.CreateClient(_restApiSettings.APIName);

            var jsonContent = new StringContent(JsonSerializer.Serialize(newCategory), Encoding.UTF8, "application/json");

            var response = await client.PostAsync(_restApiSettings.Categories, jsonContent);

            if (!response.IsSuccessStatusCode) {
                var errorResponse = await response.Content.ReadAsStringAsync();

                throw new BadRequestException($"Failed to create category. Status Code: {response.StatusCode}", errorResponse);
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var createdCategory = JsonSerializer.Deserialize<Category>(responseContent);

            return createdCategory ?? throw new Exception("Failed to deserialize created category.");

        }

        public async Task<IReadOnlyCollection<Category>> GetCategoriesAsync() {
            
            var client = _httpClientFactory.CreateClient(_restApiSettings.APIName);
            var response = await client.GetAsync(_restApiSettings.Categories);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var categories = JsonSerializer.Deserialize<List<Category>>(content);

            return categories.AsReadOnly() ?? throw new Exception("Failed to deserialize categories."); 
        }

        public async Task<Category> GetCategoryByIdAsync(int categoryId)
        {
            var client = _httpClientFactory.CreateClient(_restApiSettings.APIName);
            var response = await client.GetAsync($"{_restApiSettings.Categories}/{categoryId}");

            if (!response.IsSuccessStatusCode) {
                var errorResponse = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == HttpStatusCode.BadRequest && errorResponse.Contains("EntityNotFoundError")) {
                    throw new NotFoundException($"Category with ID {categoryId} not found.", errorResponse);
                }

                throw new Exception($"Failed to get {categoryId}. API response: {errorResponse}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var category = JsonSerializer.Deserialize<Category>(content);

            return category ?? throw new Exception("Failed to deserialize  category.");

        }

        public async Task UpdateCategoryAsync(int categoryId, CategoryMutateDto updatedCategory) {
            var client = _httpClientFactory.CreateClient(_restApiSettings.APIName);

            var requestUrl = $"{_restApiSettings.Categories}/{categoryId}";
            var jsonContent = new StringContent(JsonSerializer.Serialize(updatedCategory), Encoding.UTF8, "application/json");

            var response = await client.PutAsync(requestUrl, jsonContent);

            if (!response.IsSuccessStatusCode) {
                var errorResponse = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == HttpStatusCode.BadRequest && errorResponse.Contains("EntityNotFoundError")) {
                    throw new NotFoundException($"Category with ID {categoryId} not found.", errorResponse);
                }

                throw new Exception($"Failed to update category {categoryId}. API response: {errorResponse}");
            }
        }
    }
}
