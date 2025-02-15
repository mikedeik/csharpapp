using CSharpApp.Core.Dtos.Products;
using MediatR;


namespace CSharpApp.Application.Products.Queries {
    public record GetProductByIdQuery(int id) : IRequest<Product>;
}
