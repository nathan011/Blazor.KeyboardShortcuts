using System;

namespace Blazor.KeyboardShortcuts
{
    public class KeydownEventArgs : EventArgs
    {
        public ModKeys Mods { get; private set; }
        public KeyCodes Key { get; private set; }
        public KeydownEventArgs(ModKeys mods, KeyCodes key)
        {
            Mods = mods;
            Key = key;
        }
        public override string ToString()
        {
            return $"{Mods} {(Mods == ModKeys.None ? "" : "+")} {Key}";
        }
    }
}
