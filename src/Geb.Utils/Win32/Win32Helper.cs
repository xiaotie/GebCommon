using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Runtime.InteropServices;

namespace Geb.Utils.Win32
{
    /// <summary>Values to pass to the GetDCEx method.</summary>
    [Flags()]
    public enum DeviceContextValues : uint
    {
        /// <summary>DCX_WINDOW: Returns a DC that corresponds to the window rectangle rather 
        /// than the client rectangle.</summary>
        Window = 0x00000001,
        /// <summary>DCX_CACHE: Returns a DC from the cache, rather than the OWNDC or CLASSDC 
        /// window. Essentially overrides CS_OWNDC and CS_CLASSDC.</summary>
        Cache = 0x00000002,
        /// <summary>DCX_NORESETATTRS: Does not reset the attributes of this DC to the 
        /// default attributes when this DC is released.</summary>
        NoResetAttrs = 0x00000004,
        /// <summary>DCX_CLIPCHILDREN: Excludes the visible regions of all child windows 
        /// below the window identified by hWnd.</summary>
        ClipChildren = 0x00000008,
        /// <summary>DCX_CLIPSIBLINGS: Excludes the visible regions of all sibling windows 
        /// above the window identified by hWnd.</summary>
        ClipSiblings = 0x00000010,
        /// <summary>DCX_PARENTCLIP: Uses the visible region of the parent window. The 
        /// parent's WS_CLIPCHILDREN and CS_PARENTDC style bits are ignored. The origin is 
        /// set to the upper-left corner of the window identified by hWnd.</summary>
        ParentClip = 0x00000020,
        /// <summary>DCX_EXCLUDERGN: The clipping region identified by hrgnClip is excluded 
        /// from the visible region of the returned DC.</summary>
        ExcludeRgn = 0x00000040,
        /// <summary>DCX_INTERSECTRGN: The clipping region identified by hrgnClip is 
        /// intersected with the visible region of the returned DC.</summary>
        IntersectRgn = 0x00000080,
        /// <summary>DCX_EXCLUDEUPDATE: Unknown...Undocumented</summary>
        ExcludeUpdate = 0x00000100,
        /// <summary>DCX_INTERSECTUPDATE: Unknown...Undocumented</summary>
        IntersectUpdate = 0x00000200,
        /// <summary>DCX_LOCKWINDOWUPDATE: Allows drawing even if there is a LockWindowUpdate 
        /// call in effect that would otherwise exclude this window. Used for drawing during 
        /// tracking.</summary>
        LockWindowUpdate = 0x00000400,
        /// <summary>DCX_VALIDATE When specified with DCX_INTERSECTUPDATE, causes the DC to 
        /// be completely validated. Using this function with both DCX_INTERSECTUPDATE and 
        /// DCX_VALIDATE is identical to using the BeginPaint function.</summary>
        Validate = 0x00200000,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        private int _Left;
        private int _Top;
        private int _Right;
        private int _Bottom;

        public RECT(RECT Rectangle)
            : this(Rectangle.Left, Rectangle.Top, Rectangle.Right, Rectangle.Bottom)
        {
        }
        public RECT(int Left, int Top, int Right, int Bottom)
        {
            _Left = Left;
            _Top = Top;
            _Right = Right;
            _Bottom = Bottom;
        }

        public int X
        {
            get { return _Left; }
            set { _Left = value; }
        }
        public int Y
        {
            get { return _Top; }
            set { _Top = value; }
        }
        public int Left
        {
            get { return _Left; }
            set { _Left = value; }
        }
        public int Top
        {
            get { return _Top; }
            set { _Top = value; }
        }
        public int Right
        {
            get { return _Right; }
            set { _Right = value; }
        }
        public int Bottom
        {
            get { return _Bottom; }
            set { _Bottom = value; }
        }
        public int Height
        {
            get { return _Bottom - _Top; }
            set { _Bottom = value - _Top; }
        }
        public int Width
        {
            get { return _Right - _Left; }
            set { _Right = value + _Left; }
        }
        public Point Location
        {
            get { return new Point(Left, Top); }
            set
            {
                _Left = value.X;
                _Top = value.Y;
            }
        }
        public Size Size
        {
            get { return new Size(Width, Height); }
            set
            {
                _Right = value.Width + _Left;
                _Bottom = value.Height + _Top;
            }
        }

        public static implicit operator Rectangle(RECT Rectangle)
        {
            return new Rectangle(Rectangle.Left, Rectangle.Top, Rectangle.Width, Rectangle.Height);
        }
        public static implicit operator RECT(Rectangle Rectangle)
        {
            return new RECT(Rectangle.Left, Rectangle.Top, Rectangle.Right, Rectangle.Bottom);
        }
        public static bool operator ==(RECT Rectangle1, RECT Rectangle2)
        {
            return Rectangle1.Equals(Rectangle2);
        }
        public static bool operator !=(RECT Rectangle1, RECT Rectangle2)
        {
            return !Rectangle1.Equals(Rectangle2);
        }

        public override string ToString()
        {
            return "{Left: " + _Left + "; " + "Top: " + _Top + "; Right: " + _Right + "; Bottom: " + _Bottom + "}";
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public bool Equals(RECT Rectangle)
        {
            return Rectangle.Left == _Left && Rectangle.Top == _Top && Rectangle.Right == _Right && Rectangle.Bottom == _Bottom;
        }

        public override bool Equals(object Object)
        {
            if (Object is RECT)
            {
                return Equals((RECT)Object);
            }
            else if (Object is Rectangle)
            {
                return Equals(new RECT((Rectangle)Object));
            }

            return false;
        }
    }


    public class Win32Helper
    {
        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        public delegate bool EnumDesktopWindowsDelegate(IntPtr hWnd, int lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool EnumDesktopWindows(IntPtr hDesktop,
           EnumDesktopWindowsDelegate lpfn, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDCEx(IntPtr hWnd, IntPtr hrgnClip, DeviceContextValues flags);

        [DllImport("user32.dll")]
        public static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll", EntryPoint = "BitBlt")]
        public static extern int BitBlt(IntPtr hDestDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);
        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleBitmap")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);
        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleDC")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll", EntryPoint = "SelectObject")]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hObject);
        [DllImport("gdi32.dll", EntryPoint = "DeleteDC")]
        public static extern int DeleteDC(IntPtr hdc);
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        public static extern int DeleteObject(IntPtr hObject);
        public const int SRCCOPY = 0xCC0020;

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        public static List<Window> GetWindows(Predicate<Window> predicate = null)
        {
            List<Window> list = new List<Window>();
            EnumWindowsProc callback = (IntPtr hWnd, IntPtr lParam) =>
                {
                    int length = GetWindowTextLength(hWnd);
                    StringBuilder sb = new StringBuilder(length * 2 + 2);
                    GetWindowText(hWnd, sb, length*2);
                    Window w = new Window();
                    w.Handle = hWnd;
                    w.Title = sb.ToString();
                    if (predicate == null || predicate(w) == true)
                    {
                        list.Add(w);
                    }
                    return true;
                };
            EnumWindows(callback, IntPtr.Zero);
            return list;
        }

        public static List<Window> GetDesktopWindows(Predicate<Window> predicate = null)
        {
            List<Window> list = new List<Window>();
            EnumDesktopWindowsDelegate callback = (IntPtr hWnd, int lParam) =>
            {
                int length = GetWindowTextLength(hWnd);
                StringBuilder sb = new StringBuilder(length * 2 + 2);
                GetWindowText(hWnd, sb, length * 2);
                Window w = new Window();
                w.Handle = hWnd;
                w.Title = sb.ToString();
                if (predicate == null || predicate(w) == true)
                {
                    list.Add(w);
                }
                return true;
            };
            EnumDesktopWindows(IntPtr.Zero,callback, IntPtr.Zero);
            return list;
        }

        /// <summary>
        /// 本方法在开启Aero特效后可以截取被遮挡的窗口及最小化的窗口
        /// </summary>
        /// <param name="hwnd"></param>
        /// <returns></returns>
        public static Bitmap BitBltWindow(IntPtr hwnd)
        {
            RECT rc;
            Win32Helper.GetWindowRect(hwnd, out rc);
            if (rc.Width < 2 || rc.Height < 2) return null;

            Bitmap bmp = new Bitmap(rc.Width, rc.Height, PixelFormat.Format32bppArgb);
            Graphics gfxBmp = Graphics.FromImage(bmp);
            IntPtr hdcBitmap = gfxBmp.GetHdc();
            IntPtr hdcSrc = Win32Helper.GetDC(hwnd);
            Win32Helper.BitBlt(hdcBitmap, 0, 0, bmp.Width, bmp.Height, hdcSrc, 0, 0, 0x00CC0020);
            gfxBmp.ReleaseHdc(hdcBitmap);
            gfxBmp.Dispose();
            return bmp;
        }

        public static Bitmap BitBltWindow(IntPtr hwnd, Rectangle rect)
        {
            RECT rc;
            Win32Helper.GetWindowRect(hwnd, out rc);
            if (rc.Width < 2 || rc.Height < 2) return null;

            int xMin = 0;
            int yMin = 0;
            int xMax = rc.Width;
            int yMax = rc.Height;
            if (rect != null)
            {
                xMin = Math.Max(0, rect.X);
                yMin = Math.Max(0, rect.Y);
                xMax = Math.Min(xMax, rect.Right);
                yMax = Math.Min(yMax, rect.Bottom);
            }

            int width = xMax - xMin;
            int height = yMax - yMin;

            if (width < 1 || height < 1) return null;

            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            Graphics gfxBmp = Graphics.FromImage(bmp);
            IntPtr hdcBitmap = gfxBmp.GetHdc();
            IntPtr hdcSrc = Win32Helper.GetDC(hwnd);
            Win32Helper.BitBlt(hdcBitmap, 0, 0, bmp.Width, bmp.Height, hdcSrc, xMin, yMin, 0x00CC0020);
            gfxBmp.ReleaseHdc(hdcBitmap);
            gfxBmp.Dispose();
            return bmp;
        }

        public static Bitmap PrintWindow(IntPtr hwnd)
        {
            RECT rc;
            Win32Helper.GetWindowRect(hwnd, out rc);
            if (rc.Width < 2 || rc.Height < 2) return null;

            Bitmap bmp = new Bitmap(rc.Width, rc.Height, PixelFormat.Format32bppArgb);
            Graphics gfxBmp = Graphics.FromImage(bmp);
            IntPtr hdcBitmap = gfxBmp.GetHdc();
            bool succeeded = Win32Helper.PrintWindow(hwnd, hdcBitmap, 0);
            gfxBmp.ReleaseHdc(hdcBitmap);
            if (!succeeded)
            {
                gfxBmp.FillRectangle(new SolidBrush(Color.Gray), new Rectangle(Point.Empty, bmp.Size));
                gfxBmp.Dispose();
                bmp.Dispose();
                return null;
            }
            else
            {
                gfxBmp.Dispose();
                return bmp;
            }
        } 
    }
}
