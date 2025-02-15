
using CSharpApp.Core.Dtos.Products;
using System.Net;

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
        _restApiSettings = restApiSettings.Value;
        _logger = logger;
    }

    

    public async Task<IReadOnlyCollection<Product>> GetProductsAsync()
    {

        var client = _httpClientFactory.CreateClient("fakeapi");
        var response = await client.GetAsync(_restApiSettings.Products);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var res = JsonSerializer.Deserialize<List<Product>>(content);
        
        return res.AsReadOnly();
    }


    public async Task<Product?> GetProductByIdAsync(int id) {

        var client = _httpClientFactory.CreateClient("fakeapi");
        var response = await client.GetAsync($"{_restApiSettings.Products}/{id}");

        if (response.StatusCode == HttpStatusCode.BadRequest) {

            return null;
        }

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var res = JsonSerializer.Deserialize<Product>(content);

        return res;
    }


}