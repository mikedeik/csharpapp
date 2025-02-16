
using CSharpApp.Core.Dtos.Categories;
using CSharpApp.Core.Dtos.Products;
using CSharpApp.Core.Exceptions;
using System.Net;
using System.Text;

namespace CSharpApp.Application.Products;

public class ProductsService : IProductsService
{
    private readonly  IHttpClientFactory _httpClientFactory;
    private readonly RestApiSettings _restApiSettings;
    private readonly ILogger<ProductsService> _logger;

    public ProductsService(IOptions<RestApiSettings> restApiSettings, 
        ILogger<ProductsService> logger, IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _restApiSettings = restApiSettings.Value ?? throw new ArgumentNullException(nameof(restApiSettings));
        _logger = logger;
    }

    

    public async Task<IReadOnlyCollection<Product>> GetProductsAsync()
    {

        var client = _httpClientFactory.CreateClient("fakeapi");
        var response = await client.GetAsync(_restApiSettings.Products);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var res = JsonSerializer.Deserialize<List<Product>>(content);
        
        return res.AsReadOnly() ?? throw new Exception("Failed to deserialize products.");
    }


    public async Task<Product?> GetProductByIdAsync(int id) {

        var client = _httpClientFactory.CreateClient("fakeapi");
        var response = await client.GetAsync($"{_restApiSettings.Products}/{id}");

        if (!response.IsSuccessStatusCode) {
            var errorResponse = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == HttpStatusCode.BadRequest && errorResponse.Contains("EntityNotFoundError")) {
                throw new NotFoundException($"Product with ID {id} not found.", errorResponse);
            }

            throw new Exception($"Failed to get product with {id}. API response: {errorResponse}");
        }

        var content = await response.Content.ReadAsStringAsync();
        var product = JsonSerializer.Deserialize<Product>(content);

        return product ?? throw new Exception("Failed to deserialize  category.");
    }

    public async Task<Product> CreateProductAsync(ProductCreateDto newProduct) {

        var client = _httpClientFactory.CreateClient(_restApiSettings.APIName);
        var jsonContent = new StringContent(JsonSerializer.Serialize(newProduct), Encoding.UTF8, "application/json");

        var response = await client.PostAsync(_restApiSettings.Products, jsonContent);

        if (!response.IsSuccessStatusCode) {

            var errorResponse = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == HttpStatusCode.BadRequest && errorResponse.Contains("EntityNotFoundError")) {

                throw new BadRequestException($"Category with ID {newProduct.CategoryId} not found.", errorResponse);
            }
            throw new BadRequestException($"Failed to create product. Status Code: {response.StatusCode}", errorResponse);
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        var createdProduct = JsonSerializer.Deserialize<Product>(responseContent);
        return createdProduct ?? throw new Exception("Failed to deserialize created product");
    }

    public async Task UpdateProductAsync(int productId, ProductUpdateDto updatedProduct) {
        var client = _httpClientFactory.CreateClient(_restApiSettings.APIName);

        var requestUrl = $"{_restApiSettings.Products}/{productId}";
        var jsonContent = new StringContent(JsonSerializer.Serialize(updatedProduct), Encoding.UTF8, "application/json");

        var response = await client.PutAsync(requestUrl, jsonContent);

        if (!response.IsSuccessStatusCode) {
            var errorResponse = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == HttpStatusCode.BadRequest && errorResponse.Contains("EntityNotFoundError")) {
                throw new NotFoundException($"Product with Id {productId} was not found!", errorResponse);
            }
            throw new BadRequestException($"Failed to update product with Id {productId}.", errorResponse);
        }



    }
}