using System.Runtime.InteropServices;

namespace TGPSound.Common;

// For windows only, call native apis.
internal class WinClipboardHelper
{
    const uint CF_UNICODETEXT = 13;

    [DllImport("user32.dll")]
    private static extern bool OpenClipboard(IntPtr hWndNewOwner);

    [DllImport("user32.dll")]
    private static extern bool CloseClipboard();

    [DllImport("user32.dll")]
    private static extern IntPtr GetClipboardData(uint uFormat);

    [DllImport("user32.dll")]
    private static extern bool IsClipboardFormatAvailable(uint uFormat);

    [DllImport("kernel32.dll")]
    private static extern IntPtr GlobalLock(IntPtr hMem);

    [DllImport("kernel32.dll")]
    private static extern bool GlobalUnlock(IntPtr hMem);

    // Gets the current text content of the clipboard, or null if it's not available.
    public static string? GetClipboardString()
    {
        if (!IsClipboardFormatAvailable(CF_UNICODETEXT)) return null;
        OpenClipboard(IntPtr.Zero);
        IntPtr handle = GetClipboardData(CF_UNICODETEXT);
        if (handle == IntPtr.Zero) return null;
        IntPtr locked = GlobalLock(handle);

        if (locked == IntPtr.Zero) return null;
        string? result = Marshal.PtrToStringUni(locked);

        GlobalUnlock(handle);
        CloseClipboard();
        return result;
    }

}
