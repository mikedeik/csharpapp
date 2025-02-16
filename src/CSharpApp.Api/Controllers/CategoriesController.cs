using Asp.Versioning;
using CSharpApp.Application.Categories.Commands;
using CSharpApp.Application.Categories.Queries;
using CSharpApp.Core.Dtos.Categories;
using CSharpApp.Core.Exceptions;
using CSharpApp.Core.Settings;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CSharpApp.Api.Controllers;
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class CategoriesController: ControllerBase
{
    
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator) {

        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<Category>>> GetCategories() {

        var categories = await _mediator.Send(new GetAllCategoriesQuery());
        return Ok(categories);
    }

    [HttpGet("{categoryId}")]
    public async Task<ActionResult<IReadOnlyCollection<Category>>> GetCategoryById(int categoryId) {

        var category = await _mediator.Send(new GetCategoryByIdQuery(categoryId));
        return Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult<Category>> CreateCategory([FromBody] CategoryMutateDto newCategory) {
     
        var category = await _mediator.Send(new CreateCategoryCommand(newCategory));
        return Created($"/categories/{category.Id}",category);
    }

    [HttpPut("{categoryId}")]
    public async Task<IActionResult> UpdateCategory(int categoryId, [FromBody] CategoryMutateDto updatedCategory) {

        await _mediator.Send(new UpdateCategoryCommand(categoryId, updatedCategory));

        return NoContent(); // 204 No Content (best practice for updates)
    }
}