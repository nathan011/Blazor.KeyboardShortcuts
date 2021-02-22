namespace Blazor.KeyboardShortcuts
{
    public enum AllowIn
    {
        /// <summary>
        /// Default. Everywhere but text entry elements: INPUT (text) and TEXTAREA.
        /// </summary>
        No_Text_Input = 0,

        /// <summary>
        /// Allow anywhere.
        /// </summary>
        Anywhere = 1,
    }
}
