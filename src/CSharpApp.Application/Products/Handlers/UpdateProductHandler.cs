using CSharpApp.Application.Products.Commands;
using MediatR;

namespace CSharpApp.Application.Products.Handlers {
    public class UpdateProductHandler: IRequestHandler<UpdateProductCommand> {

        private readonly IProductsService _productsService;

        public UpdateProductHandler(IProductsService productsService) {
            _productsService = productsService;
        }

        public async Task Handle(UpdateProductCommand request, CancellationToken token) {

            await _productsService.UpdateProductAsync(request.ProductId, request.ProductUpdateDto);
        }
    }
}
