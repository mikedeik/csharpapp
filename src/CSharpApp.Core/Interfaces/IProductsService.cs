using CSharpApp.Core.Dtos.Products;

namespace CSharpApp.Core.Interfaces;

public interface IProductsService
{
    Task<IReadOnlyCollection<Product>> GetProductsAsync();
    Task<Product> GetProductByIdAsync(int id);
    Task<Product> CreateProductAsync(ProductCreateDto newProduct);
    Task UpdateProductAsync(int productId, ProductUpdateDto updatedProduct);
}