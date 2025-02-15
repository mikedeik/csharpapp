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
            try {
                var products = await _mediator.Send(new GetAllProductsQuery());
                return Ok(products);

            }catch(Exception ex) {
                return BadRequest(new ProblemDetails {
                    Title = "Product page not available",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = ex.Message,
                    Instance = HttpContext.Request.Path
                });
            }

        }

        [HttpGet("{productId}")]
        public async Task<ActionResult<IReadOnlyCollection<Product>>> GetProducts(int productId) {

            try {

                var product = await _mediator.Send(new GetProductByIdQuery(productId));

                if (product == null) {
                    return NotFound(new ProblemDetails {
                        Title = "Product Not Found",
                        Status = StatusCodes.Status404NotFound,
                        Detail = $"Product with ID {productId} was not found.",
                        Instance = HttpContext.Request.Path
                    });
                }

                return Ok(product);

            } catch (Exception ex) {
                return BadRequest(new ProblemDetails {
                    Title = "Product Not Found",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = ex.Message,
                    Instance = HttpContext.Request.Path
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductMutateDto newProduct) {

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
