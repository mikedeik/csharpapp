using CSharpApp.Application.Categories.Queries;
using CSharpApp.Core.Dtos.Categories;
using CSharpApp.Core.Exceptions;
using MediatR;

namespace CSharpApp.Application.Categories.Handlers;

public class GetAllCategoriesHandler: IRequestHandler<GetAllCategoriesQuery, IReadOnlyCollection<Category>>
{
    private readonly ICategoriesService _categoriesService;
    private readonly ICacheService _cacheService;
    private readonly CacheSettings _cacheSettings;

    public GetAllCategoriesHandler(ICategoriesService categoriesService, ICacheService cacheService, IOptions<CacheSettings> options)
    {
        _categoriesService = categoriesService ?? throw new ArgumentNullException(nameof(categoriesService));
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService)); 
        _cacheSettings = options.Value ?? throw new ArgumentNullException(nameof(options));
    }
    public async Task<IReadOnlyCollection<Category>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken) {

        return await _cacheService.GetOrCreateAsync(
            _cacheSettings.CategoriesKey,
            async () => await _categoriesService.GetCategoriesAsync(),
            TimeSpan.FromMinutes(_cacheSettings.CacheMinutesDurationCategories)
            );
        
       
    }
}