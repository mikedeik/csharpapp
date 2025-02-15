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

        // Defining the timeout policy
        var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(httpClientSettings.LifeTime));

        // Using Polly to implement Retry policy
        // Added package Polly.Contrib.WaitAndRetry to get the backoff configuration implemented by polly
        services.AddHttpClient("fakeapi", client => {
            client.BaseAddress = new Uri(httpClientBaseUrl);
        }).AddTransientHttpErrorPolicy(policyBuilder =>
            policyBuilder.WaitAndRetryAsync(
                Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromMilliseconds(httpClientSettings.SleepDuration), httpClientSettings.RetryCount))
            ).AddPolicyHandler(timeoutPolicy);

        return services;
    }
}