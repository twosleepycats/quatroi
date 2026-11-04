using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace QuatRoi
{
    internal static class LayeredWindow
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct Blend { public byte Operation, Flags, Alpha, Format; }

        internal static void Present(IntPtr window, Bitmap bitmap, Point position)
        {
            IntPtr screen = GetDC(IntPtr.Zero);
            IntPtr memory = IntPtr.Zero;
            IntPtr handle = IntPtr.Zero;
            IntPtr previous = IntPtr.Zero;
            try
            {
                memory = CreateCompatibleDC(screen);
                handle = bitmap.GetHbitmap(Color.FromArgb(0));
                previous = SelectObject(memory, handle);
                var size = bitmap.Size;
                var source = Point.Empty;
                var blend = new Blend { Alpha = 255, Format = 1 };
                if (!UpdateLayeredWindow(window, screen, ref position, ref size, memory, ref source, 0, ref blend, 2))
                    throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            finally
            {
                if (previous != IntPtr.Zero) SelectObject(memory, previous);
                if (handle != IntPtr.Zero) DeleteObject(handle);
                if (memory != IntPtr.Zero) DeleteDC(memory);
                if (screen != IntPtr.Zero) ReleaseDC(IntPtr.Zero, screen);
            }
        }

        [DllImport("user32.dll", SetLastError = true)] private static extern bool UpdateLayeredWindow(IntPtr window, IntPtr screen, ref Point position, ref Size size, IntPtr source, ref Point sourcePosition, uint key, ref Blend blend, uint flags);
        [DllImport("user32.dll")] private static extern IntPtr GetDC(IntPtr window);
        [DllImport("user32.dll")] private static extern int ReleaseDC(IntPtr window, IntPtr dc);
        [DllImport("gdi32.dll")] private static extern IntPtr CreateCompatibleDC(IntPtr dc);
        [DllImport("gdi32.dll")] private static extern bool DeleteDC(IntPtr dc);
        [DllImport("gdi32.dll")] private static extern IntPtr SelectObject(IntPtr dc, IntPtr obj);
        [DllImport("gdi32.dll")] private static extern bool DeleteObject(IntPtr obj);
    }
}
