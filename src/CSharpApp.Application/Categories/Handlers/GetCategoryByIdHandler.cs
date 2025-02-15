using CSharpApp.Application.Categories.Queries;
using CSharpApp.Core.Dtos.Categories;
using CSharpApp.Core.Exceptions;
using MediatR;

namespace CSharpApp.Application.Categories.Handlers;

public class GetCategoryByIdHandler: IRequestHandler<GetCategoryByIdQuery, Category>
{
     private readonly ICategoriesService _categoriesService;

     public GetCategoryByIdHandler(ICategoriesService categoriesService)
     {
          _categoriesService = categoriesService ?? throw new ArgumentNullException(nameof(categoriesService));
     }
     
     public async Task<Category> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken) {
        
          try {

               var category = await _categoriesService.GetCategoryByIdAsync(request.id);

               if (category == null) throw new NotFoundException($"Category with id {request.id} was not found");
               return category;
          } catch (Exception ex)
          {
               throw;
          }
     }
    
}