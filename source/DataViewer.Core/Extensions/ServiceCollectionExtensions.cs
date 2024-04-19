using DataViewer.Core.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace DataViewer.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services.AddSingleton<IIOProvider, IOProvider>();
        services.AddSingleton<IDataExtractor, MachineDataExtractor>();

        return services;
    }
}