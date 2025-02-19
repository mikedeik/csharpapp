using CSharpApp.Application;
using CSharpApp.Application.Authentication;
using CSharpApp.Application.Categories;
using CSharpApp.Infrastructure.Services;

namespace CSharpApp.Infrastructure.Configuration;

public static class DefaultConfiguration
{
    public static IServiceCollection AddDefaultConfiguration(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var configuration = serviceProvider.GetService<IConfiguration>();

        // We should ensure validation of the settings from the appsettings.json file
        // For this reason we use the AddOptions with validation

        services.AddOptions<RestApiSettings>()
            .Bind(configuration!.GetSection(nameof(RestApiSettings)))
            .ValidateDataAnnotations() 
            .ValidateOnStart();

        services.AddOptions<HttpClientSettings>()
            .Bind(configuration.GetSection(nameof(HttpClientSettings)))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<AuthenticationSettings>()
            .Bind(configuration.GetSection(nameof(AuthenticationSettings)))
            .ValidateDataAnnotations()
            .Validate(settings => {
                return settings.Key.Length >= 32;
            }, "The Key for JWT tokens needs to be at least 32 characters long.")
            .ValidateOnStart();

        services.AddOptions<CacheSettings>()
            .Bind(configuration.GetSection(nameof(CacheSettings)))
            .ValidateDataAnnotations()
            .Validate(settings => {
                // Custom validation the Products and Categories keys should be diferent
                return settings.ProductsKey != settings.CategoriesKey;
            },"ProductsKey and CategoriesKey must be different.")
            .ValidateOnStart();

        services.AddSingleton<ICacheService, CacheService>();
        services.AddTransient<IProductsService, ProductsService>();
        services.AddTransient<ICategoriesService, CategoriesService>();
        services.AddTransient<IAuthenticationService, AuthenticationService>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(IApplicationMarker).Assembly));

        return services;
    }
}