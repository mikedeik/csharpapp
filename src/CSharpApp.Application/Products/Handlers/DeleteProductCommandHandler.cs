using CSharpApp.Application.Products.Commands;
using CSharpApp.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpApp.Application.Products.Handlers {
    public class DeleteProductCommandHandler: IRequestHandler<DeleteProductCommad> {
        
        private readonly IProductsService _productsService;
        private readonly ICacheService _cacheService;
        private readonly CacheSettings _cacheSettings;

        public DeleteProductCommandHandler(IProductsService productsService, ICacheService cacheService, IOptions<CacheSettings> options) {
            _productsService = productsService ?? throw new ArgumentNullException(nameof(productsService));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
            _cacheSettings = options.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task Handle(DeleteProductCommad request, CancellationToken cancellationToken) {

            await _productsService.DeleteProductAsync(request.productId);
            _cacheService?.Remove(_cacheSettings.ProductsKey);
            _cacheService?.Remove($"{_cacheSettings.ProductsKey}_{request.productId}");
        }
    }
}
