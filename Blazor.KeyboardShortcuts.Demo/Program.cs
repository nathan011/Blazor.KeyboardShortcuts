using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Blazor.KeyboardShortcuts.Demo
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.Services.AddKeyboardShortcuts();
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            var host = builder.Build();
            var nav = host.Services.GetService<NavigationManager>();
            var shortcuts = host.Services.GetService<KeyboardShortcutsService>();
            shortcuts.CreateGlobalChord(new KeyboardShortcutChord(ModKeys.None, KeyCodes.G, ModKeys.None, KeyCodes.H, () => nav.NavigateTo(nav.BaseUri), "go to home page"));
            shortcuts.CreateGlobalChord(new KeyboardShortcutChord(ModKeys.None, KeyCodes.G, ModKeys.None, KeyCodes.C, () => nav.NavigateTo(nav.BaseUri + "counter"), "go to counter page"));
            shortcuts.CreateGlobalChord(new KeyboardShortcutChord(ModKeys.None, KeyCodes.G, ModKeys.None, KeyCodes.F, () => nav.NavigateTo(nav.BaseUri + "fetchdata"), "go to fetch data  page"));
            await host.RunAsync();
        }
    }
}
