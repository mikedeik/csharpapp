using CSharpApp.Application.Categories.Queries;
using CSharpApp.Core.Dtos.Categories;
using MediatR;

namespace CSharpApp.Application.Categories.Handlers;

public class GetCategoryByIdHandler: IRequestHandler<GetCategoryByIdQuery, Category>
{
    private readonly ICategoriesService _categoriesService;
    private readonly ICacheService _cacheService;
    private readonly CacheSettings _cacheSettings;

    public GetCategoryByIdHandler(ICategoriesService categoriesService, ICacheService cacheService, IOptions<CacheSettings> options)
     {
        _categoriesService = categoriesService ?? throw new ArgumentNullException(nameof(categoriesService));
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        _cacheSettings = options.Value ?? throw new ArgumentNullException(nameof(options));
     }
     
     public async Task<Category> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken) {

        return await _cacheService.GetOrCreateAsync(
             $"{_cacheSettings.CategoriesKey}_{request.id}", 
             async () => await _categoriesService.GetCategoryByIdAsync(request.id),
             TimeSpan.FromMinutes(_cacheSettings.CacheMinutesDurationCategories)
            );
     }
    
}