﻿namespace Orbital7.Extensions.Integrations.SlackApi;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddSlackApi(
        this IServiceCollection services,
        string apiToken)
    {
        services.AddScoped<IChatApi, ChatApi>(
            (serviceProvider) => new ChatApi(
                new SlackApiClient(
                    serviceProvider.GetRequiredService<IHttpClientFactory>(),
                    apiToken)));

        return services;
    }
}
