using Microsoft.Extensions.DependencyInjection;

namespace SkillExtractor.Domain;

public static class DependencyInjection
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        // Domain services registered here as features are added
        return services;
    }
}
