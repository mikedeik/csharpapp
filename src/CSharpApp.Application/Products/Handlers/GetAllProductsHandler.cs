using CSharpApp.Application.Products.Queries;
using CSharpApp.Core.Dtos.Products;
using CSharpApp.Core.Exceptions;
using MediatR;


namespace CSharpApp.Application.Products.Handlers {
    public class GetAllProductsHandler : IRequestHandler<GetAllProductsQuery, IReadOnlyCollection<Product>> {
        
        private readonly IProductsService _productsService;
        private readonly ICacheService _cacheService;
        private readonly CacheSettings _cacheSettings;

        public GetAllProductsHandler(IProductsService productService, ICacheService cacheService, IOptions<CacheSettings> options) {
            _productsService = productService ?? throw new ArgumentNullException(nameof(productService));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
            _cacheSettings = options.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<IReadOnlyCollection<Product>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken) {

            return await _cacheService.GetOrCreateAsync(
                _cacheSettings.ProductsKey,
                async () => await _productsService.GetProductsAsync(),
                TimeSpan.FromMinutes(_cacheSettings.CahceMinutesDurationProducts)
                );

        }
    }
}
