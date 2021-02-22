using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Blazor.KeyboardShortcuts
{
    public static class KeyboardShortcutsServiceExtensions
    {
        public static IServiceCollection AddKeyboardShortcuts(this IServiceCollection services)
        {
            services.AddScoped(serviceProvider => new KeyboardShortcutsService(serviceProvider.GetService<IJSRuntime>()));
            //services.AddScoped(serviceProvider => new KeyboardShortcutsService(serviceProvider.GetService<IJSRuntime>(), serviceProvider.GetService<ILogger<KeyboardShortcutsService>>()));
            return services;
        }
    }
}
