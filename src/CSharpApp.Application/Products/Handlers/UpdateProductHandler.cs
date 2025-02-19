using CSharpApp.Application.Products.Commands;
using MediatR;

namespace CSharpApp.Application.Products.Handlers {
    public class UpdateProductHandler: IRequestHandler<UpdateProductCommand> {

        private readonly IProductsService _productsService;
        private readonly ICacheService _cacheService;
        private readonly CacheSettings _cacheSettings;

        public UpdateProductHandler(IProductsService productsService, ICacheService cacheService, IOptions<CacheSettings> options) {
            _productsService = productsService ?? throw new ArgumentNullException(nameof(productsService));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
            _cacheSettings = options.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task Handle(UpdateProductCommand request, CancellationToken token) {

            await _productsService.UpdateProductAsync(request.ProductId, request.ProductUpdateDto);
            _cacheService.Remove(_cacheSettings.ProductsKey);
            _cacheService.Remove($"{_cacheSettings.ProductsKey}_{request.ProductId}");

        }
    }
}
