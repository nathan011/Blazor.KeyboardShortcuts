using System;

namespace Blazor.KeyboardShortcuts
{
    [Flags]
    public enum ModKeys
    {
        None = 0,
        Shift = 1,
        Control = 2,
        Shift_Control = Shift | Control,
        Alt = 4,
        Shift_Alt = Shift | Alt,
        Shift_Alt_Control = Shift | Control | Alt,
        Control_Alt = Control | Alt,
        Meta = 8
    }
}
