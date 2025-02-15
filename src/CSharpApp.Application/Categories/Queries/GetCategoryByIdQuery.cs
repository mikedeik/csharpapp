using CSharpApp.Core.Dtos.Categories;
using MediatR;

namespace CSharpApp.Application.Categories.Queries;

public record GetCategoryByIdQuery(int id): IRequest<Category>;