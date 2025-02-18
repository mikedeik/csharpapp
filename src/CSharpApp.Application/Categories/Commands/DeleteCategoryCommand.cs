using MediatR;


namespace CSharpApp.Application.Categories.Commands {
    public record DeleteCategoryCommand(int categoryId) : IRequest;
}
