using Asp.Versioning;
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
        public async Task<ActionResult<IReadOnlyCollection<Product>>> GetProducts(int productId) {

            
            var product = await _mediator.Send(new GetProductByIdQuery(productId));
            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductCreateDto newProduct) {

            if (!ModelState.IsValid) {
                return BadRequest(Problem(
                            title: "Validation Error",
                            detail: "Some fields have incorrect values.",
                            statusCode: StatusCodes.Status400BadRequest,
                            extensions: new Dictionary<string, object?>
                            {
                                { "errors", ModelState }
                            }
                 ));
            }

            return Ok("Product created!");
        }
    }
}
