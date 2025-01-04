using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VideoUploader.Core.Interfaces;
using VideoUploader.Core.Services;
using VideoUploader.Infrastructure.Services;

namespace VideoUploader.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpClient<IBunnyStorageService, BunnyStorageService>();
            
        services.AddScoped<IVideoUploadService, VideoUploadService>();

        return services;
    }
}
