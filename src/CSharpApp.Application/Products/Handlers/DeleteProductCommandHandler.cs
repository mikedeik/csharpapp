using CSharpApp.Application.Products.Commands;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpApp.Application.Products.Handlers {
    public class DeleteProductCommandHandler: IRequestHandler<DeleteProductCommad> {
        
        private readonly IProductsService _productsService;

        public DeleteProductCommandHandler(IProductsService productsService) {
            _productsService = productsService;
        }

        public async Task Handle(DeleteProductCommad request, CancellationToken cancellationToken) {

            await _productsService.DeleteProductAsync(request.productId);
        }
    }
}
