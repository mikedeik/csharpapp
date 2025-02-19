using CSharpApp.Application.Products.Queries;
using CSharpApp.Core.Dtos.Products;
using CSharpApp.Core.Exceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpApp.Application.Products.Handlers {
    public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, Product> {

        private readonly IProductsService _productsService;
        private readonly ICacheService _cacheService;
        private readonly CacheSettings _cacheSettings;


        public GetProductByIdHandler(IProductsService productService, ICacheService cacheService, IOptions<CacheSettings> options) {
            _productsService = productService ?? throw new ArgumentNullException(nameof(productService));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
            _cacheSettings = options.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<Product> Handle(GetProductByIdQuery request, CancellationToken cancellationToken) {


            return await _cacheService.GetOrCreateAsync(
                $"{_cacheSettings.ProductsKey}_{request.id}",
                async () => await _productsService.GetProductByIdAsync(request.id),
                TimeSpan.FromMinutes(_cacheSettings.CahceMinutesDurationProducts)
            );

        }
    }
}
