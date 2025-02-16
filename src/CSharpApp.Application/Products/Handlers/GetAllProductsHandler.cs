using CSharpApp.Application.Products.Queries;
using CSharpApp.Core.Dtos.Products;
using CSharpApp.Core.Exceptions;
using MediatR;


namespace CSharpApp.Application.Products.Handlers {
    public class GetAllProductsHandler : IRequestHandler<GetAllProductsQuery, IReadOnlyCollection<Product>> {
        
        private readonly IProductsService _productsService;

        public GetAllProductsHandler(IProductsService productService) {
            _productsService = productService;
        }

        public async Task<IReadOnlyCollection<Product>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken) {

            return await _productsService.GetProductsAsync();
        }
    }
}
