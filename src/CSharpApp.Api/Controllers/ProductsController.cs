using Asp.Versioning;
using CSharpApp.Application.Products.Commands;
using CSharpApp.Application.Products.Queries;
using CSharpApp.Core.Dtos.Products;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CSharpApp.Api.Controllers {

    [ApiController]
    [Route("api/v{version:apiVersion}/products")]
    [ApiVersion("1.0")]
    public class ProductsController : ControllerBase {
        
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator) {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyCollection<Product>>> GetProducts() {
            

            var products = await _mediator.Send(new GetAllProductsQuery());
            return Ok(products);

        }

        [HttpGet("{productId}")]
        public async Task<ActionResult<IReadOnlyCollection<Product>>> GetProductById(int productId) {

            
            var product = await _mediator.Send(new GetProductByIdQuery(productId));
            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductCreateDto newProduct) {

            var product = await _mediator.Send(new CreateProductCommand(newProduct));
            return CreatedAtAction(nameof(GetProductById), new { productId = product.Id}, product);
        }

        [HttpPut("{productId}")]
        public async Task<IActionResult> UpdateProduct(int productId, [FromBody] ProductUpdateDto updatedProduct) {


            await _mediator.Send(new UpdateProductCommand(productId, updatedProduct));
            return NoContent();
        }

    }
}
