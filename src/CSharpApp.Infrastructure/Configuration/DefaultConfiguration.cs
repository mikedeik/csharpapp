using CSharpApp.Application;
using CSharpApp.Application.Authentication;
using CSharpApp.Application.Categories;

namespace CSharpApp.Infrastructure.Configuration;

public static class DefaultConfiguration
{
    public static IServiceCollection AddDefaultConfiguration(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var configuration = serviceProvider.GetService<IConfiguration>();

        services.Configure<RestApiSettings>(configuration!.GetSection(nameof(RestApiSettings)));
        services.Configure<HttpClientSettings>(configuration.GetSection(nameof(HttpClientSettings)));
        services.Configure<AuthenticationSettings>(configuration.GetSection(nameof(AuthenticationSettings)));

        services.AddTransient<IProductsService, ProductsService>();
        services.AddTransient<ICategoriesService, CategoriesService>();
        services.AddTransient<IAuthenticationService, AuthenticationService>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(IApplicationMarker).Assembly));

        return services;
    }
}