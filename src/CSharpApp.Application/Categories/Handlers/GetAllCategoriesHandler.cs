using CSharpApp.Application.Categories.Queries;
using CSharpApp.Core.Dtos.Categories;
using CSharpApp.Core.Exceptions;
using MediatR;

namespace CSharpApp.Application.Categories.Handlers;

public class GetAllCategoriesHandler: IRequestHandler<GetAllCategoriesQuery, IReadOnlyCollection<Category>>
{
    private readonly ICategoriesService _categoriesService;
    public GetAllCategoriesHandler(ICategoriesService categoriesService)
    {
        _categoriesService = categoriesService ?? throw new ArgumentNullException(nameof(categoriesService));
    }
    public async Task<IReadOnlyCollection<Category>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken) {
        
        try {

            return await _categoriesService.GetCategoriesAsync();
            
        } catch (Exception ex) {
            throw new NotFoundException("Categories page not available");
        }
    }
}