using CSharpApp.Application.Products.Commands;
using CSharpApp.Core.Dtos.Products;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpApp.Application.Products.Handlers {
    public class CreateProductHandler : IRequestHandler<CreateProductCommand, Product> {

        private readonly IProductsService _productsService;

        public CreateProductHandler(IProductsService productsService) {
            _productsService = productsService;
        }

        public async Task<Product> Handle(CreateProductCommand request, CancellationToken token) {

            var product = await _productsService.CreateProductAsync(request.ProductCreateDto);

            return product;
        }
    }
}
