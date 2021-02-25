# Blazor.KeyboardShortcuts
A library to add keyboard shortcuts and chords to Blazor WASM projects (does NOT work with Blazor server side).

Demo available at [https://nathan011.github.io/Blazor.KeyboardShortcuts](https://nathan011.github.io/Blazor.KeyboardShortcuts/).

Inspired by [Toolbelt.Blazor.HotKeys](https://github.com/jsakamoto/Toolbelt.Blazor.HotKeys), but with a few additional features, and the application logic in C#, rather than javascript.

### Usage
1) Install [nuget package](https://www.nuget.org/packages/BlazorKeyboardShortcuts)

2) In Program.cs add the service to the DI builder.
```C#
builder.Services.AddKeyboardShortcuts();
```

3) (Optional) In Program.cs, create global shortcuts/chords (such as navigation commands):
```C#
var host = builder.Build();
var nav = host.Services.GetService<NavigationManager>();
var shortcuts = host.Services.GetService<KeyboardShortcutsService>();
shortcuts.CreateGlobalChord(new KeyboardShortcutChord(ModKeys.None, KeyCodes.G, ModKeys.None, KeyCodes.H, () => nav.NavigateTo(nav.BaseUri), "go to home page"));
shortcuts.CreateGlobalChord(new KeyboardShortcutChord(ModKeys.None, KeyCodes.G, ModKeys.None, KeyCodes.C, () => nav.NavigateTo(nav.BaseUri + "counter"), "go to counter page"));
shortcuts.CreateGlobalChord(new KeyboardShortcutChord(ModKeys.None, KeyCodes.G, ModKeys.None, KeyCodes.F, () => nav.NavigateTo(nav.BaseUri + "fetchdata"), "go to fetch data  page"));
```
4) Inject the service into pages:
```C#
@inject KeyboardShortcutsService kb
```

5) Use injected service to create shortcuts:
```C#
protected override Task OnInitializedAsync()
    {
        kb.CreateReplaceContextShortcuts(
            new[]
            {
                new KeyboardShortcut(ModKeys.None, KeyCodes.J, sel_next),
                new KeyboardShortcut(ModKeys.None, KeyCodes.K, sel_prev),
                new KeyboardShortcut(ModKeys.None, KeyCodes.DownArrow, sel_next),
                new KeyboardShortcut(ModKeys.None, KeyCodes.UpArrow, sel_prev),
                new KeyboardShortcut(ModKeys.None, KeyCodes.Tab, toggle_table, prevent_default: true),
            });
        return base.OnInitializedAsync();
    }
 ```
