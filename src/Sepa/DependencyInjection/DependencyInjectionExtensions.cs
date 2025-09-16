using Microsoft.Extensions.DependencyInjection;
using Toarnbeike.Sepa.Writers;

namespace Toarnbeike.Sepa.DependencyInjection;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddSepaWriter(this IServiceCollection services)
    {
        services.AddSingleton<ISepaPain00800108Writer, SepaPain00800108Writer>();
        return services;
    }
}