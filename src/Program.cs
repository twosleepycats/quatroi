using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Media;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace QuatRoi
{
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            using (var instance = new Mutex(false, @"Local\QuatRoi.Desktop.SingleInstance"))
            {
                bool ownsInstance;
                try { ownsInstance = instance.WaitOne(0); }
                catch (AbandonedMutexException) { ownsInstance = true; }
                if (!ownsInstance) return;
                try { Run(args); }
                finally { instance.ReleaseMutex(); }
            }
        }

        private static void Run(string[] args)
        {
            // Keep cursor, mouse-hook and layered-window positions in physical pixels
            // across monitors with different scaling and negative desktop coordinates.
            try
            {
                if (!SetProcessDpiAwarenessContext(new IntPtr(-4))) SetProcessDPIAware();
            }
            catch (EntryPointNotFoundException) { SetProcessDPIAware(); }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (var atlas = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("whip-atlas.png")))
            {
                if (args.Length > 0 && args[0] == "--self-test")
                {
                    if (atlas.Width != atlas.Height || atlas.Width < 64) throw new Exception("Invalid atlas grid");
                    if (atlas.GetPixel(0, 0).A > 8) throw new Exception("Atlas background must be transparent");
                    using (var player = new SoundPlayer(Assembly.GetExecutingAssembly().GetManifestResourceStream("whip-crack.wav"))) player.Load();
                    using (var layer = new Bitmap(520, 520, PixelFormat.Format32bppPArgb))
                    using (var layerGraphics = Graphics.FromImage(layer))
                    {
                        var cursor = new Point(300, 250);
                        var bounds = Overlay.WhipBounds(cursor, 230);
                        if (Math.Abs(bounds.X + 230 * 0.83 - cursor.X) > 1 || Math.Abs(bounds.Y + 230 * 0.85 - cursor.Y) > 1)
                            throw new Exception("Whip grip must track cursor");
                        layerGraphics.Clear(Color.Transparent);
                        Overlay.DrawFrame(layerGraphics, atlas, 5, bounds);
                        if (layer.GetPixel(0, 0).A != 0) throw new Exception("Overlay background must have zero alpha");
                        layer.Save("art/overlay-preview.png");
                    }
                    using (var preview = new Bitmap(800, 400))
                    using (var g = Graphics.FromImage(preview))
                    {
                        g.Clear(Color.FromArgb(32, 36, 44));
                        Overlay.DrawFrame(g, atlas, 5, new Rectangle(30, 30, 320, 320));
                        Overlay.DrawFrame(g, atlas, 11, new Rectangle(420, 30, 320, 320));
                        preview.Save("art/preview.png");
                    }
                    return;
                }
                Application.Run(new Overlay(atlas, args.Length > 0 && args[0] == "--smoke"));
            }
        }

        [DllImport("user32.dll")] private static extern bool SetProcessDpiAwarenessContext(IntPtr context);
        [DllImport("user32.dll")] private static extern bool SetProcessDPIAware();
    }

    internal sealed class Overlay : Form
    {
        private readonly Bitmap atlas;
        private readonly System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer { Interval = 16 };
        private readonly Stopwatch clock = Stopwatch.StartNew();
        private readonly NotifyIcon tray;
        private readonly Icon appIcon;
        private readonly ToolStripMenuItem toggle;
        private readonly HookProc hookProc;
        private IntPtr hook;
        private bool active;
        private bool swinging;
        private Point target;
        private long swingStart;
        private bool f8Down;
        private bool escapeDown;
        private bool showShortcutDown;
        private readonly bool smoke;
        private Point lastCursor = new Point(int.MinValue, int.MinValue);
        private readonly Bitmap surface = new Bitmap(520, 520, PixelFormat.Format32bppPArgb);
        private readonly SoundPlayer sound = new SoundPlayer(Assembly.GetExecutingAssembly().GetManifestResourceStream("whip-crack.wav"));
        private bool sounded;
        private bool muted;

        public Overlay(Bitmap image, bool smokeTest)
        {
            atlas = image;
            smoke = smokeTest;
            FormBorderStyle = FormBorderStyle.None;
            AutoScaleMode = AutoScaleMode.None;
            ShowInTaskbar = false;
            TopMost = true;
            DoubleBuffered = true;
            Size = surface.Size;
            sound.Load();
            using (var iconStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("app.ico"))
            using (var resourceIcon = new Icon(iconStream)) appIcon = (Icon)resourceIcon.Clone();
            Icon = appIcon;
            Text = "Quat Roi";
            var menu = new ContextMenuStrip();
            toggle = new ToolStripMenuItem("Hien roi (Ctrl+Shift+Home)", null, delegate { Toggle(); });
            menu.Items.Add(toggle);
            var muteItem = new ToolStripMenuItem("Tat am thanh") { CheckOnClick = true };
            muteItem.CheckedChanged += delegate { muted = muteItem.Checked; if (muted) sound.Stop(); };
            menu.Items.Add(muteItem);
            menu.Items.Add("Thoat hoan toan", null, delegate { Close(); });
            tray = new NotifyIcon { Icon = appIcon, Text = "Quat Roi - Ctrl+Shift+Home: hien | Esc: an", ContextMenuStrip = menu, Visible = true };
            tray.DoubleClick += delegate { Toggle(); };
            hookProc = OnMouse;
            timer.Tick += delegate
            {
                bool nextF8 = (GetAsyncKeyState(0x77) & 0x8000) != 0;
                bool nextEscape = (GetAsyncKeyState(0x1B) & 0x8000) != 0;
                bool nextShow = ShowShortcutPressed();
                if (nextF8 && !f8Down) Toggle();
                if (nextShow && !showShortcutDown) SetActive(true);
                if (nextEscape && !escapeDown) SetActive(false);
                f8Down = nextF8;
                escapeDown = nextEscape;
                showShortcutDown = nextShow;
                bool wasSwinging = swinging;
                if (swinging && clock.ElapsedMilliseconds - swingStart >= 560) swinging = false;
                Point current = Cursor.Position;
                if (active && (wasSwinging || current != lastCursor))
                {
                    RenderLayer();
                }
                if (swinging && !sounded && clock.ElapsedMilliseconds - swingStart >= 280)
                {
                    sounded = true;
                    if (!muted) sound.Play();
                }
                lastCursor = current;
                if (smoke && clock.ElapsedMilliseconds > 1200) Close();
            };
            DisplaySettingsChangedHandler = delegate { RenderLayer(); };
            Microsoft.Win32.SystemEvents.DisplaySettingsChanged += DisplaySettingsChangedHandler;
        }

        private readonly EventHandler DisplaySettingsChangedHandler;
        protected override bool ShowWithoutActivation { get { return true; } }
        protected override CreateParams CreateParams
        {
            get
            {
                var p = base.CreateParams;
                p.ExStyle |= 0x00080000 | 0x00000020 | 0x08000000 | 0x00000080;
                return p;
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            f8Down = (GetAsyncKeyState(0x77) & 0x8000) != 0;
            escapeDown = (GetAsyncKeyState(0x1B) & 0x8000) != 0;
            showShortcutDown = ShowShortcutPressed();
            hook = SetWindowsHookEx(14, hookProc, GetModuleHandle(null), 0);
            if (hook == IntPtr.Zero)
            {
                StartupError("Khong the khoi tao chuot. Win32 error: " + Marshal.GetLastWin32Error());
                Close();
                return;
            }
            timer.Start();
            if (smoke)
            {
                muted = true;
                SetActive(true);
                swinging = true;
                swingStart = clock.ElapsedMilliseconds;
                target = new Point(Cursor.Position.X - 168, Cursor.Position.Y - 12);
            }
            RenderLayer();
            if (!smoke) tray.ShowBalloonTip(4000, "Quat Roi", "Ctrl+Shift+Home: hien roi. Esc: an roi. Thoat hoan toan bang menu khay he thong.", ToolTipIcon.Info);
        }

        private void StartupError(string message)
        {
            if (smoke)
            {
                System.IO.File.WriteAllText("art/smoke-error.txt", message);
                Environment.ExitCode = 1;
            }
            else MessageBox.Show(message, "Quat Roi");
        }

        private void Toggle()
        {
            SetActive(!active);
        }

        private static bool ShowShortcutPressed()
        {
            return (GetAsyncKeyState(0x11) & 0x8000) != 0
                && (GetAsyncKeyState(0x10) & 0x8000) != 0
                && (GetAsyncKeyState(0x24) & 0x8000) != 0;
        }

        private void SetActive(bool value)
        {
            if (active == value) return;
            active = value;
            swinging = false;
            sound.Stop();
            toggle.Text = active ? "An roi (Esc)" : "Hien roi (Ctrl+Shift+Home)";
            tray.Text = active ? "Quat Roi - DANG BAT | Esc: an roi" : "Quat Roi - DANG AN | Ctrl+Shift+Home: hien";
            RenderLayer();
        }

        private bool swallowingClick;
        private IntPtr OnMouse(int code, IntPtr message, IntPtr data)
        {
            if (code >= 0)
            {
                int msg = message.ToInt32();
                if (msg == 0x0202 && swallowingClick)
                {
                    swallowingClick = false;
                    return (IntPtr)1;
                }
                var mouse = (MouseInfo)Marshal.PtrToStructure(data, typeof(MouseInfo));
                if (active && msg == 0x0201)
                {
                    swallowingClick = true;
                    target = new Point(mouse.Position.X - 168, mouse.Position.Y - 12);
                    swingStart = clock.ElapsedMilliseconds;
                    swinging = true;
                    sounded = false;
                    return (IntPtr)1;
                }
            }
            return CallNextHookEx(hook, code, message, data);
        }

        private void RenderLayer()
        {
            Point screenCursor = Cursor.Position;
            Point origin = new Point(screenCursor.X - 300, screenCursor.Y - 250);
            using (var g = Graphics.FromImage(surface))
            {
                g.Clear(Color.Transparent);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                if (active)
                {
                    Point cursor = new Point(300, 250);
                    int elapsed = (int)(clock.ElapsedMilliseconds - swingStart);
                    int frame = swinging ? Math.Min(7, elapsed / 70) : 0;
                    DrawFrame(g, atlas, frame, WhipBounds(cursor, 230));
                    if (swinging && elapsed >= 280)
                    {
                        int impact = Math.Min(7, (elapsed - 280) / 35);
                        DrawFrame(g, atlas, 8 + impact, new Rectangle(target.X - origin.X - 80, target.Y - origin.Y - 80, 160, 160));
                    }
                }
            }
            LayeredWindow.Present(Handle, surface, origin);
        }

        internal static void DrawFrame(Graphics g, Bitmap image, int index, Rectangle destination)
        {
            float cell = image.Width / 4f;
            var source = new RectangleF(index % 4 * cell, index / 4 * cell, cell, cell);
            if (index < 8)
            {
                var state = g.Save();
                try
                {
                    g.TranslateTransform(destination.Right, destination.Top);
                    g.ScaleTransform(-1, 1);
                    g.DrawImage(image, new Rectangle(0, 0, destination.Width, destination.Height), source, GraphicsUnit.Pixel);
                }
                finally { g.Restore(state); }
            }
            else g.DrawImage(image, destination, source, GraphicsUnit.Pixel);
        }

        internal static Rectangle WhipBounds(Point cursor, int size)
        {
            return new Rectangle(cursor.X - (int)(size * 0.83), cursor.Y - (int)(size * 0.85), size, size);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                timer.Stop();
                if (hook != IntPtr.Zero) { UnhookWindowsHookEx(hook); hook = IntPtr.Zero; }
                Microsoft.Win32.SystemEvents.DisplaySettingsChanged -= DisplaySettingsChangedHandler;
                timer.Dispose();
                sound.Stop();
                sound.Dispose();
                surface.Dispose();
                tray.Visible = false;
                var menu = tray.ContextMenuStrip;
                tray.Dispose();
                appIcon.Dispose();
                menu.Dispose();
            }
            base.Dispose(disposing);
        }

        private delegate IntPtr HookProc(int code, IntPtr message, IntPtr data);
        [StructLayout(LayoutKind.Sequential)]
        private struct MouseInfo { public Point Position; public uint MouseData; public uint Flags; public uint Time; public IntPtr Extra; }
        [DllImport("user32.dll", SetLastError = true)] private static extern IntPtr SetWindowsHookEx(int id, HookProc proc, IntPtr module, uint thread);
        [DllImport("user32.dll")] private static extern bool UnhookWindowsHookEx(IntPtr handle);
        [DllImport("user32.dll")] private static extern IntPtr CallNextHookEx(IntPtr handle, int code, IntPtr message, IntPtr data);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)] private static extern IntPtr GetModuleHandle(string name);
        [DllImport("user32.dll")] private static extern short GetAsyncKeyState(int key);
    }
}
