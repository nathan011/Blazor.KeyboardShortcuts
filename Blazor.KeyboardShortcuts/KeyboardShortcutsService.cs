using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blazor.KeyboardShortcuts
{
    public class KeyboardShortcutsService
    {
        IJSRuntime js;
        bool chord_started;
        (ModKeys mods, KeyCodes key) chord_start_keys;
        //ILogger<KeyboardShortcutsService> logger;
        Dictionary<(int, int), DateTime> throttle_dict = new();
        Dictionary<(ModKeys, KeyCodes), KeyboardShortcut> global_shortcuts = new();
        Dictionary<(ModKeys, KeyCodes), KeyboardShortcut> context_shortcuts = new();
        Dictionary<(ModKeys, KeyCodes), Dictionary<(ModKeys, KeyCodes), KeyboardShortcutChord>> global_chords = new Dictionary<(ModKeys, KeyCodes), Dictionary<(ModKeys, KeyCodes), KeyboardShortcutChord>>();
        Dictionary<(ModKeys, KeyCodes), Dictionary<(ModKeys, KeyCodes), KeyboardShortcutChord>> context_chords = new Dictionary<(ModKeys, KeyCodes), Dictionary<(ModKeys, KeyCodes), KeyboardShortcutChord>>();
        public KeyboardShortcutsService(IJSRuntime jSRuntime)
        //public KeyboardShortcutsService(IJSRuntime jSRuntime, ILogger<KeyboardShortcutsService> logger)
        {
            js = jSRuntime;
            AttachEvent();
        }
        public event EventHandler<KeydownEventArgs> Keydown;
        public bool Disabled { get; set; }
        public int ThrottleMilliseconds { get; set; }
        public IEnumerable<KeyboardShortcut> GlobalShortcuts { get => global_shortcuts.Values; }
        public IEnumerable<KeyboardShortcutChord> GlobalShortcutChords { get => global_chords.Values.SelectMany(v => v.Values); }
        public IEnumerable<KeyboardShortcut> ContextShortcuts { get => context_shortcuts.Values; }
        public IEnumerable<KeyboardShortcutChord> ContextShortcutChords { get => context_chords.Values.SelectMany(v => v.Values); }
        public KeyboardShortcutsService CreateGlobalShortcut(KeyboardShortcut shortcut)
        {
            global_shortcuts[(shortcut.Mods, shortcut.Key)] = shortcut;
            return this;
        }
        public KeyboardShortcutsService CreateGlobalChord(KeyboardShortcutChord chord)
        {
            if (global_chords.TryGetValue((chord.Mods, chord.Key), out var dict))
            {
                dict[(chord.Mods2, chord.Key2)] = chord;
            }
            else
            {
                global_chords[(chord.Mods, chord.Key)] = new Dictionary<(ModKeys, KeyCodes), KeyboardShortcutChord>(new[] { KeyValuePair.Create((chord.Mods2, chord.Key2), chord) });
            }
            return this;
        }
        public void RemoveGlobalShortcut(KeyboardShortcut shortcut)
        {
            global_shortcuts.Remove((shortcut.Mods, shortcut.Key));
        }
        public void RemoveGlobalChord(KeyboardShortcutChord chord)
        {
            if (global_chords.TryGetValue((chord.Mods, chord.Key), out var chords))
            {
                chords.Remove((chord.Mods2, chord.Key2));
            }
        }
        public void CreateReplaceContextShortcuts(IEnumerable<KeyboardShortcut> shortcuts)
        {
            context_shortcuts.Clear();
            if (shortcuts == null) return;
            foreach (var shortcut in shortcuts)
            {
                context_shortcuts.Add((shortcut.Mods, shortcut.Key), shortcut);
            }
        }
        public void CreateReplaceContextChords(IEnumerable<KeyboardShortcutChord> chords)
        {
            context_chords.Clear();
            if (chords == null) return;
            foreach (var chord in chords)
            {
                context_chords[(chord.Mods, chord.Key)] = new Dictionary<(ModKeys, KeyCodes), KeyboardShortcutChord>(new[] { KeyValuePair.Create((chord.Mods2, chord.Key2), chord) });
            }
        }
        public void Clear()
        {
            ClearGlobal();
            ClearContext();
        }
        public void ClearGlobal()
        {
            global_shortcuts.Clear();
            global_chords.Clear();
        }
        public void ClearContext()
        {
            context_shortcuts.Clear();
            context_chords.Clear();
        }
        async void AttachEvent()
        {
            var service_ref = DotNetObjectReference.Create(this);
            await js.InvokeVoidAsync("window.eval", keydown_script);
            await js.InvokeVoidAsync("attach_kb_service", service_ref);
        }
        [JSInvokable]
        public bool DotnetOnkeydown(int modkeys, string key, int keyCode, string tagName, string type)
        {
            if (Disabled) return false;
            if (ThrottleMilliseconds > 0)
            {
                if (throttle_dict.TryGetValue((modkeys, keyCode), out var last_keydown) && DateTime.Now.Subtract(last_keydown).TotalMilliseconds < ThrottleMilliseconds)
                {
                    return false;
                }
                throttle_dict[(modkeys, keyCode)] = DateTime.Now;
            }
            var prevent_default = false;
            //logger.LogDebug($"keydown: {modkeys}, {key}, {keyCode}, {tagName}, {type}");
            Enum.TryParse<ModKeys>(modkeys.ToString(), out var mods);
            Enum.TryParse<KeyCodes>(keyCode.ToString(), out var keycode);
            Keydown?.Invoke(this, new KeydownEventArgs(mods, keycode));
            if (IsModifierKey(keycode)) return false;
            if (chord_started)
            {
                if (global_chords.TryGetValue((chord_start_keys), out var chords))
                {
                    if (chords.TryGetValue((mods, keycode), out var chord) && (chord.AllowIn == AllowIn.Anywhere || !IsTextInput(tagName, type)))
                    {
                        //logger.LogDebug($"global chord shortcut: {chord.Description}");
                        chord.Action.Invoke();
                        prevent_default = chord.PreventDefault;
                    }
                }
                if (context_chords.TryGetValue((chord_start_keys), out chords))
                {
                    if (chords.TryGetValue((mods, keycode), out var chord) && (chord.AllowIn == AllowIn.Anywhere || !IsTextInput(tagName, type)))
                    {
                        //logger.LogDebug($"context chord shortcut: {chord.Description}");
                        chord.Action.Invoke();
                        prevent_default = chord.PreventDefault;
                    }
                }
                chord_started = false;
            }
            else
            {
                if (global_shortcuts.TryGetValue((mods, keycode), out var shortcut) && (shortcut.AllowIn == AllowIn.Anywhere || !IsTextInput(tagName, type)))
                {
                    //logger.LogDebug($"global shortcut: {shortcut.Description}");
                    shortcut.Action.Invoke();
                    prevent_default = shortcut.PreventDefault;
                }
                else if (context_shortcuts.TryGetValue((mods, keycode), out shortcut) && (shortcut.AllowIn == AllowIn.Anywhere || !IsTextInput(tagName, type)))
                {
                    //logger.LogDebug($"context shortcut: {shortcut.Description}");
                    shortcut.Action.Invoke();
                    prevent_default = shortcut.PreventDefault;
                }
                else if (global_chords.ContainsKey((mods, keycode)) || context_chords.ContainsKey((mods, keycode)) && (shortcut.AllowIn == AllowIn.Anywhere || !IsTextInput(tagName, type)))
                {
                    chord_started = true;
                    chord_start_keys = (mods, keycode);
                }
            }
            return prevent_default;
        }
        bool IsModifierKey(KeyCodes keycode)
        {
            return keycode == KeyCodes.Shift || keycode == KeyCodes.Ctrl || keycode == KeyCodes.Alt || keycode == KeyCodes.LeftWindowKey || keycode == KeyCodes.RightWindowKey;
        }
        bool IsTextInput(string tag_name, string input_type)
        {
            return
                (string.Equals(tag_name, "INPUT", StringComparison.OrdinalIgnoreCase) && string.Equals(input_type, "text", StringComparison.OrdinalIgnoreCase))
                || string.Equals(tag_name, "TEXTAREA", StringComparison.OrdinalIgnoreCase);
        }
        const string keydown_script = @"
        window.attach_kb_service = function(kbs) {
            document.addEventListener('keydown', async ev => {
                if (typeof (ev['altKey']) === 'undefined') return;
                var modKeys = (ev.shiftKey ? 1 : 0) +
                        (ev.ctrlKey ? 2 : 0) +
                        (ev.altKey ? 4 : 0) +
                        (ev.metaKey ? 8 : 0);
                const key = ev.key;
                const keyCode = ev.keyCode;
                const tagName = ev.srcElement.tagName;
                const type = ev.srcElement.getAttribute('type');
                const prevent_default = await kbs.invokeMethodAsync('DotnetOnkeydown', modKeys, key, keyCode, tagName, type);
                if (prevent_default === true) ev.preventDefault();
            });
        }";
    }
}
