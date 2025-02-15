using Asp.Versioning;
using CSharpApp.Application.Products.Queries;
using CSharpApp.Core.Dtos.Products;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CSharpApp.Api.Controllers;
[ApiController]
[Route("api/v{version:apiVersion}/categories")]
[ApiVersion("1.0")]
public class CategoriesController: ControllerBase
{
    
}