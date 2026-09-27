using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soenneker.X.Users.Abstract;
using Soenneker.X.ClientUtil.Registrars;

namespace Soenneker.X.Users.Registrars;

/// <summary>
/// Utilities for X users and creating posts.
/// </summary>
public static class XUsersUtilRegistrar
{
    /// <summary>
    /// Adds <see cref="IXUsersUtil"/> as a singleton service. <para/>
    /// </summary>
    public static IServiceCollection AddXUsersUtilAsSingleton(this IServiceCollection services)
    {
        services.AddXClientUtilAsSingleton().TryAddSingleton<IXUsersUtil, XUsersUtil>();

        return services;
    }

    /// <summary>
    /// Adds <see cref="IXUsersUtil"/> as a scoped service. <para/>
    /// </summary>
    public static IServiceCollection AddXUsersUtilAsScoped(this IServiceCollection services)
    {
        services.AddXClientUtilAsScoped().TryAddScoped<IXUsersUtil, XUsersUtil>();

        return services;
    }
}

