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

        public GetProductByIdHandler(IProductsService productService) {
            _productsService = productService;
        }

        public async Task<Product> Handle(GetProductByIdQuery request, CancellationToken cancellationToken) {

            var product = await _productsService.GetProductByIdAsync(request.id);
            return product;
        }
    }
}
