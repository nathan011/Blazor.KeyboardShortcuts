using System;

namespace Blazor.KeyboardShortcuts
{
    public class KeyboardShortcutChord
    {
        public KeyboardShortcutChord(ModKeys mods, KeyCodes key, ModKeys mods2, KeyCodes key2, Action action, string desctiption = null, bool prevent_default = false, AllowIn allowin = AllowIn.No_Text_Input)
        {
            Mods = mods;
            Key = key;
            Mods2 = mods2;
            Key2 = key2;
            Action = action;
            Description = desctiption;
            PreventDefault = prevent_default;
            AllowIn = allowin;
        }
        public ModKeys Mods { get; set; }
        public KeyCodes Key { get; set; }
        public ModKeys Mods2 { get; set; }
        public KeyCodes Key2 { get; set; }
        public Action Action { get; set; }
        public string Description { get; set; }
        public bool PreventDefault { get; set; }
        public AllowIn AllowIn { get; set; }
    }
}
