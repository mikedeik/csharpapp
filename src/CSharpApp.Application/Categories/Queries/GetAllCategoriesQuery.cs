using CSharpApp.Core.Dtos.Categories;
using MediatR;

namespace CSharpApp.Application.Categories.Queries;

public record GetAllCategoriesQuery() : IRequest<IReadOnlyCollection<Category>>;