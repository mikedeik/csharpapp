using Polly.Extensions.Http;
using Polly;
using Polly.Contrib.WaitAndRetry;

namespace CSharpApp.Infrastructure.Configuration;

public static class HttpConfiguration
{
    public static IServiceCollection AddHttpConfiguration(this IServiceCollection services, IConfiguration configuration)
    {

        var httpClientSettings = configuration.GetSection("HttpClientSettings").Get<HttpClientSettings>();
        var httpClientBaseUrl = configuration.GetSection("RestApiSettings:BaseUrl").Get<string>();
        var restApiName = configuration.GetSection("RestApiSettings:APIName").Get<string>();

        // Define a timeout policy
        var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(httpClientSettings.LifeTime));

        // Using Polly to implement Retry policy
        // Added package Polly.Contrib.WaitAndRetry 
        services.AddHttpClient(restApiName, client => {
            client.BaseAddress = new Uri(httpClientBaseUrl);
        }).AddTransientHttpErrorPolicy(policyBuilder =>
            policyBuilder.WaitAndRetryAsync(
                Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromMilliseconds(httpClientSettings.SleepDuration), httpClientSettings.RetryCount))
            ).AddPolicyHandler(timeoutPolicy);

        return services;
    }
}