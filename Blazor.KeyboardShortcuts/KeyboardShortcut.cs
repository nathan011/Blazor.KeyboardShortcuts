using System;

namespace Blazor.KeyboardShortcuts
{
    public class KeyboardShortcut
    {
        public KeyboardShortcut(ModKeys mods, KeyCodes key, Action action, string desctiption = null, bool prevent_default = false, AllowIn allowin = AllowIn.No_Text_Input)
        {
            Mods = mods;
            Key = key;
            Action = action;
            Description = desctiption;
            PreventDefault = prevent_default;
            AllowIn = allowin;
        }
        public ModKeys Mods { get; set; }
        public KeyCodes Key { get; set; }
        public Action Action { get; set; }
        public string Description { get; set; }
        public bool PreventDefault { get; set; }
        public AllowIn AllowIn { get; set; }
    }
}
