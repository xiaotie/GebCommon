using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Reflection;

namespace Geb.Utils.WinForm
{
    /// <summary>
    /// 改自 http://www.codeproject.com/Articles/37296/RCM-v-Theming-library-Customize-the-Appearance
    /// </summary>
    public abstract class ScrollBarSkin : NativeWindow, IDisposable
    {
        #region Constants
        // style
        private const int GWL_STYLE = (-16);
        private const int GWL_EXSTYLE = (-20);
        private const int WS_EX_TOPMOST = 0x8;
        private const int WS_EX_TOOLWINDOW = 0x80;
        private const int WS_CHILD = 0x40000000;
        private const int WS_OVERLAPPED = 0x0;
        private const int WS_CLIPSIBLINGS = 0x4000000;
        private const int WS_VISIBLE = 0x10000000;
        private const int WS_HSCROLL = 0x100000;
        private const int WS_VSCROLL = 0x200000;
        private const int SS_OWNERDRAW = 0xD;
        // showwindow
        private const int SW_HIDE = 0x0;
        private const int SW_NORMAL = 0x1;
        // size/move
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOZORDER = 0x0004;
        private const uint SWP_NOREDRAW = 0x0008;
        private const uint SWP_NOACTIVATE = 0x0010;
        private const uint SWP_FRAMECHANGED = 0x0020;
        private const uint SWP_SHOWWINDOW = 0x0040;
        private const uint SWP_HIDEWINDOW = 0x0080;
        private const uint SWP_NOCOPYBITS = 0x0100;
        private const uint SWP_NOOWNERZORDER = 0x0200;
        private const uint SWP_NOSENDCHANGING = 0x0400;
        // setwindowpos
        static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        static readonly IntPtr HWND_TOP = new IntPtr(0);
        static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        // scroll messages
        private const int WM_HSCROLL = 0x114;
        private const int WM_VSCROLL = 0x115;
        private const int SB_LINEUP = 0;
        private const int SB_LINEDOWN = 1;
        private const int SB_LINELEFT = 0;
        private const int SB_LINERIGHT = 1;
        private const int SB_PAGEUP = 2;
        private const int SB_PAGEDOWN = 3;
        private const int SB_PAGELEFT = 2;
        private const int SB_PAGERIGHT = 3;
        // mouse buttons
        private const int VK_LBUTTON = 0x1;
        private const int VK_RBUTTON = 0x2;
        // redraw
        private const int RDW_INVALIDATE = 0x0001;
        private const int RDW_INTERNALPAINT = 0x0002;
        private const int RDW_ERASE = 0x0004;
        private const int RDW_VALIDATE = 0x0008;
        private const int RDW_NOINTERNALPAINT = 0x0010;
        private const int RDW_NOERASE = 0x0020;
        private const int RDW_NOCHILDREN = 0x0040;
        private const int RDW_ALLCHILDREN = 0x0080;
        private const int RDW_UPDATENOW = 0x0100;
        private const int RDW_ERASENOW = 0x0200;
        private const int RDW_FRAME = 0x0400;
        private const int RDW_NOFRAME = 0x0800;
        // scroll bar messages
        private const int SB_HORZ = 0x0;
        private const int SB_VERT = 0x1;
        private const int SBM_SETPOS = 0x00E0;
        private const int SBM_GETPOS = 0x00E1;
        private const int SBM_SETRANGE = 0x00E2;
        private const int SBM_SETRANGEREDRAW = 0x00E6;
        private const int SBM_GETRANGE = 0x00E3;
        private const int SBM_ENABLE_ARROWS = 0x00E4;
        private const int SBM_SETSCROLLINFO = 0x00E9;
        private const int SBM_GETSCROLLINFO = 0x00EA;
        private const int SBM_GETSCROLLBARINFO = 0x00EB;
        private const int SIF_RANGE = 0x0001;
        private const int SIF_PAGE = 0x0002;
        private const int SIF_POS = 0x0004;
        private const int SIF_DISABLENOSCROLL = 0x0008;
        private const int SIF_TRACKPOS = 0x0010;
        private const int SIF_ALL = (SIF_RANGE | SIF_PAGE | SIF_POS | SIF_TRACKPOS);
        // scrollbar states
        private const int STATE_SYSTEM_INVISIBLE = 0x00008000;
        private const int STATE_SYSTEM_OFFSCREEN = 0x00010000;
        private const int STATE_SYSTEM_PRESSED = 0x00000008;
        private const int STATE_SYSTEM_UNAVAILABLE = 0x00000001;
        private const uint OBJID_HSCROLL = 0xFFFFFFFA;
        private const uint OBJID_VSCROLL = 0xFFFFFFFB;
        private const uint OBJID_CLIENT = 0xFFFFFFFC;
        // window messages
        private const int WM_PAINT = 0xF;
        private const int WM_NCPAINT = 0x85;
        private const int WM_NCMOUSEMOVE = 0xA0;
        private const int WM_MOUSEMOVE = 0x200;
        private const int WM_MOUSELEAVE = 0x2A3;
        private const int WM_LBUTTONDOWN = 0x201;
        private const int WM_LBUTTONUP = 0x202;
        private const int WM_LBUTTONDBLCLK = 0x203;
        private const int WM_RBUTTONDOWN = 0x204;
        private const int WM_RBUTTONUP = 0x205;
        private const int WM_RBUTTONDBLCLK = 0x206;
        private const int WM_MBUTTONDOWN = 0x207;
        private const int WM_MBUTTONUP = 0x208;
        private const int WM_MBUTTONDBLCLK = 0x209;
        private const int WM_MOUSEWHEEL = 0x20A;
        private const int WM_STYLECHANGED = 0x7D;
        private const int WM_SIZE = 0x5;
        private const int WM_MOVE = 0x3;
        // message handler
        private static IntPtr MSG_HANDLED = new IntPtr(1);
        #endregion

        #region Enums
        private enum SB_HITEST : int
        {
            offControl = 0,
            topArrow,
            bottomArrow,
            leftArrow,
            rightArrow,
            button,
            track
        }

        private enum SYSTEM_METRICS : int
        {
            SM_CXSCREEN = 0,
            SM_CYSCREEN = 1,
            SM_CXVSCROLL = 2,
            SM_CYHSCROLL = 3,
            SM_CYCAPTION = 4,
            SM_CXBORDER = 5,
            SM_CYBORDER = 6,
            SM_CYVTHUMB = 9,
            SM_CXHTHUMB = 10,
            SM_CXICON = 11,
            SM_CYICON = 12,
            SM_CXCURSOR = 13,
            SM_CYCURSOR = 14,
            SM_CYMENU = 15,
            SM_CXFULLSCREEN = 16,
            SM_CYFULLSCREEN = 17,
            SM_CYKANJIWINDOW = 18,
            SM_MOUSEPRESENT = 19,
            SM_CYVSCROLL = 20,
            SM_CXHSCROLL = 21,
            SM_SWAPBUTTON = 23,
            SM_CXMIN = 28,
            SM_CYMIN = 29,
            SM_CXSIZE = 30,
            SM_CYSIZE = 31,
            SM_CXFRAME = 32,
            SM_CYFRAME = 33,
            SM_CXMINTRACK = 34,
            SM_CYMINTRACK = 35,
            SM_CYSMCAPTION = 51,
            SM_CXMINIMIZED = 57,
            SM_CYMINIMIZED = 58,
            SM_CXMAXTRACK = 59,
            SM_CYMAXTRACK = 60,
            SM_CXMAXIMIZED = 61,
            SM_CYMAXIMIZED = 62
        }
        #endregion

        protected bool _isFocus = false;

        #region 扩展接口

        protected abstract void DrawVerticalScrollBar(Graphics g,
            int scrollBarWidth, int scrollBarHeight,
            int thumbTop, int thumbBottom, int arrowSize, bool isMouseOver, bool isClick);

        protected abstract void DrawSizer(Graphics g, int sizerWidth, int sizerHeight);

        protected abstract void DrawHorizontalScrollBar(Graphics g,
            int scrollBarWidth, int scrollBarHeight,
            int thumbTop, int thumbBottom, int arrowSize, bool isMouseOver, bool isClick);

        #endregion

        #region Structs
        [StructLayout(LayoutKind.Sequential)]
        private struct PAINTSTRUCT
        {
            internal IntPtr hdc;
            internal int fErase;
            internal RECT rcPaint;
            internal int fRestore;
            internal int fIncUpdate;
            internal int Reserved1;
            internal int Reserved2;
            internal int Reserved3;
            internal int Reserved4;
            internal int Reserved5;
            internal int Reserved6;
            internal int Reserved7;
            internal int Reserved8;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            internal RECT(int X, int Y, int Width, int Height)
            {
                this.Left = X;
                this.Top = Y;
                this.Right = Width;
                this.Bottom = Height;
            }
            internal int Left;
            internal int Top;
            internal int Right;
            internal int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SCROLLINFO
        {
            internal uint cbSize;
            internal uint fMask;
            internal int nMin;
            internal int nMax;
            internal uint nPage;
            internal int nPos;
            internal int nTrackPos;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SCROLLBARINFO
        {
            internal int cbSize;
            internal RECT rcScrollBar;
            internal int dxyLineButton;
            internal int xyThumbTop;
            internal int xyThumbBottom;
            internal int reserved;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            internal int[] rgstate;
        }
        #endregion

        #region API
        [DllImport("user32.dll")]
        private static extern IntPtr BeginPaint(IntPtr hWnd, ref PAINTSTRUCT ps);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EndPaint(IntPtr hWnd, ref PAINTSTRUCT ps);

        [DllImport("gdi32.dll")]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hObject);

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, int dwRop);

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool StretchBlt(IntPtr hDest, int X, int Y, int nWidth, int nHeight, IntPtr hdcSrc,
        int sX, int sY, int nWidthSrc, int nHeightSrc, int dwRop);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr handle);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr handle, IntPtr hdc);

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, ref SCROLLBARINFO lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        [DllImport("user32.dll")]
        private extern static int OffsetRect(ref RECT lpRect, int x, int y);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ValidateRect(IntPtr hWnd, ref RECT lpRect);

        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(SYSTEM_METRICS smIndex);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool RedrawWindow(IntPtr hWnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, uint flags);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(ref Point lpPoint);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool PtInRect([In] ref RECT lprc, Point pt);

        [DllImport("user32.dll")]
        private static extern int ScreenToClient(IntPtr hwnd, ref Point lpPoint);

        [DllImport("user32.dll")]
        private static extern short GetKeyState(int nVirtKey);

        [DllImport("user32.dll")]
        private static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);

        [DllImport("user32.dll")]
        private static extern int GetScrollPos(IntPtr hWnd, int nBar);

        [DllImport("user32.dll")]
        private static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr CreateWindowEx(int exstyle, string lpClassName, string lpWindowName, int dwStyle,
            int x, int y, int nWidth, int nHeight, IntPtr hwndParent, IntPtr Menu, IntPtr hInstance, IntPtr lpParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DestroyWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndAfter, int x, int y, int cx, int cy, uint flags);


        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EqualRect([In] ref RECT lprc1, [In] ref RECT lprc2);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hwnd, int nIndex);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern int GetScrollBarInfo(IntPtr hWnd, uint idObject, ref SCROLLBARINFO psbi);

        #endregion

        #region Fields
        private bool _bTrackingMouse = false;
        private int _iArrowCx = 0;
        private int _iArrowCy = 0;
        private IntPtr _hVerticalMaskWnd = IntPtr.Zero;
        private IntPtr _hHorizontalMaskWnd = IntPtr.Zero;
        private IntPtr _hSizerMaskWnd = IntPtr.Zero;
        private IntPtr _hControlWnd = IntPtr.Zero;
        #endregion

        #region Constructor
        public ScrollBarSkin(IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero) throw new Exception("The control handle is invalid.");
            ScrollbarMetrics();
            _hControlWnd = hWnd;
            CreateScrollBarMask();
            this.AssignHandle(_hControlWnd);
        }
        #endregion

        #region Properties

        private int HScrollPos
        {
            get { return GetScrollPos((IntPtr)this.Handle, SB_HORZ); }
            set { SetScrollPos((IntPtr)this.Handle, SB_HORZ, value, true); }
        }

        private int VScrollPos
        {
            get { return GetScrollPos((IntPtr)this.Handle, SB_VERT); }
            set { SetScrollPos((IntPtr)this.Handle, SB_VERT, value, true); }
        }

        #endregion

        #region Methods
        private void CheckBarState()
        {
            if ((GetWindowLong(_hControlWnd, GWL_STYLE) & WS_VISIBLE) == WS_VISIBLE)
            {
                if (HasHorizontal())
                    ShowWindow(_hHorizontalMaskWnd, SW_NORMAL);
                else
                    ShowWindow(_hHorizontalMaskWnd, SW_HIDE);

                if (HasVertical())
                    ShowWindow(_hVerticalMaskWnd, SW_NORMAL);
                else
                    ShowWindow(_hVerticalMaskWnd, SW_HIDE);

                if (HasSizer())
                    ShowWindow(_hSizerMaskWnd, SW_NORMAL);
                else
                    ShowWindow(_hSizerMaskWnd, SW_HIDE);
            }
            else
            {
                ShowWindow(_hHorizontalMaskWnd, SW_HIDE);
                ShowWindow(_hVerticalMaskWnd, SW_HIDE);
                ShowWindow(_hSizerMaskWnd, SW_HIDE);
            }
        }

        private void CreateScrollBarMask()
        {
            Type t = typeof(ScrollBarSkin);
            Module m = t.Module;
            IntPtr hInstance = Marshal.GetHINSTANCE(m);
            IntPtr hParent = GetParent(_hControlWnd);
            RECT tr = new RECT();
            Point pt = new Point();
            SCROLLBARINFO sb = new SCROLLBARINFO();
            sb.cbSize = Marshal.SizeOf(sb);

            // vertical scrollbar
            // get the size and position
            GetScrollBarInfo(_hControlWnd, OBJID_VSCROLL, ref sb);
            tr = sb.rcScrollBar;
            pt.X = tr.Left;
            pt.Y = tr.Top;
            ScreenToClient(hParent, ref pt);

            // create the window
            _hVerticalMaskWnd = CreateWindowEx(WS_EX_TOPMOST | WS_EX_TOOLWINDOW,
                "STATIC", "",
                SS_OWNERDRAW | WS_CHILD | WS_CLIPSIBLINGS | WS_OVERLAPPED | WS_VISIBLE,
                pt.X, pt.Y,
                (tr.Right - tr.Left), (tr.Bottom - tr.Top),
                hParent,
                IntPtr.Zero, hInstance, IntPtr.Zero);

            // set z-order
            SetWindowPos(_hVerticalMaskWnd, HWND_TOP,
                0, 0,
                0, 0,
                SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE | SWP_NOOWNERZORDER);

            // horizontal scrollbar
            GetScrollBarInfo(_hControlWnd, OBJID_HSCROLL, ref sb);
            tr = sb.rcScrollBar;
            pt.X = tr.Left;
            pt.Y = tr.Top;
            ScreenToClient(hParent, ref pt);

            _hHorizontalMaskWnd = CreateWindowEx(WS_EX_TOPMOST | WS_EX_TOOLWINDOW,
                "STATIC", "",
                SS_OWNERDRAW | WS_CHILD | WS_CLIPSIBLINGS | WS_OVERLAPPED | WS_VISIBLE,
                pt.X, pt.Y,
                (tr.Right - tr.Left), (tr.Bottom - tr.Top),
                hParent,
                IntPtr.Zero, hInstance, IntPtr.Zero);

            SetWindowPos(_hHorizontalMaskWnd, HWND_TOP,
                0, 0,
                0, 0,
                SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE | SWP_NOOWNERZORDER);

            // sizer
            _hSizerMaskWnd = CreateWindowEx(WS_EX_TOPMOST | WS_EX_TOOLWINDOW,
                "STATIC", "",
                SS_OWNERDRAW | WS_CHILD | WS_CLIPSIBLINGS | WS_OVERLAPPED | WS_VISIBLE,
                pt.X + (tr.Right - tr.Left), pt.Y,
                _iArrowCx, _iArrowCy,
                hParent,
                IntPtr.Zero, hInstance, IntPtr.Zero);

            SetWindowPos(_hSizerMaskWnd, HWND_TOP,
                0, 0,
                0, 0,
                SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE | SWP_NOOWNERZORDER);
            ResizeMask();
        }

        private void DrawScrollBar()
        {
            RECT tr = new RECT();
            IntPtr hdc = IntPtr.Zero;
            SCROLLBARINFO sb = new SCROLLBARINFO();
            sb.cbSize = Marshal.SizeOf(sb);

            if (HasHorizontal())
            {
                GetScrollBarInfo(_hControlWnd, OBJID_HSCROLL, ref sb);
                tr = sb.rcScrollBar;
                OffsetRect(ref tr, -tr.Left, -tr.Top);
                hdc = GetDC(_hHorizontalMaskWnd);
                Boolean isMouseOver = HitTest(Orientation.Horizontal);
                Boolean isClick = IsLeftKeyPressed();
                if (isMouseOver == true) _isFocus = true;
                using (Graphics g = Graphics.FromHdc(hdc))
                {
                    DrawHorizontalScrollBar(g, tr.Right, tr.Bottom, sb.xyThumbTop, sb.xyThumbBottom, _iArrowCx,
                        isMouseOver, isClick);
                }
                ReleaseDC(_hHorizontalMaskWnd, hdc);
            }

            if (HasSizer())
            {
                hdc = GetDC(_hSizerMaskWnd);
                using (Graphics g = Graphics.FromHdc(hdc))
                {
                    DrawSizer(g, _iArrowCx, _iArrowCy);
                }
                ReleaseDC(_hSizerMaskWnd, hdc);
            }

            if (HasVertical())
            {
                GetScrollBarInfo(_hControlWnd, OBJID_VSCROLL, ref sb);
                tr = sb.rcScrollBar;
                OffsetRect(ref tr, -tr.Left, -tr.Top);
                Boolean isMouseOver = HitTest(Orientation.Vertical);
                Boolean isClick = IsLeftKeyPressed();
                if (isMouseOver == true) _isFocus = true;
                hdc = GetDC(_hVerticalMaskWnd);
                using (Graphics g = Graphics.FromHdc(hdc))
                {
                    DrawVerticalScrollBar(g, tr.Right, tr.Bottom, sb.xyThumbTop, sb.xyThumbBottom, _iArrowCy,
                        isMouseOver, isClick);
                }
                ReleaseDC(_hVerticalMaskWnd, hdc);
            }
        }

        private Boolean HitTest(Orientation orient)
        {
            Point pt = new Point();
            RECT tr = new RECT();
            RECT tp = new RECT();
            SCROLLBARINFO sb = new SCROLLBARINFO();
            sb.cbSize = Marshal.SizeOf(sb);

            GetCursorPos(ref pt);

            if (orient == Orientation.Horizontal)
            {
                GetScrollBarInfo(_hControlWnd, OBJID_HSCROLL, ref sb);
                tr = sb.rcScrollBar;
                tp = tr;
                if (PtInRect(ref tr, pt))
                {
                    return true;
                }
            }
            else
            {
                GetScrollBarInfo(_hControlWnd, OBJID_VSCROLL, ref sb);
                tr = sb.rcScrollBar;
                tp = tr;
                if (PtInRect(ref tr, pt))
                {
                    return true;
                }
            }
            return false;
        }

        private bool HasHorizontal()
        {
            return ((GetWindowLong(_hControlWnd, GWL_STYLE) & WS_HSCROLL) == WS_HSCROLL);
        }

        private bool HasSizer()
        {
            return (HasHorizontal() && HasVertical());
        }

        private bool HasVertical()
        {
            return ((GetWindowLong(_hControlWnd, GWL_STYLE) & WS_VSCROLL) == WS_VSCROLL);
        }

        private void InvalidateWindow(bool messaged)
        {
            if (messaged)
                RedrawWindow(_hControlWnd, IntPtr.Zero, IntPtr.Zero, RDW_INTERNALPAINT);
            else
                RedrawWindow(_hControlWnd, IntPtr.Zero, IntPtr.Zero, RDW_INVALIDATE | RDW_UPDATENOW);
        }

        private bool IsLeftKeyPressed()
        {
            if (IsMouseButtonsSwitched())
                return (GetKeyState(VK_RBUTTON) < 0);
            else
                return (GetKeyState(VK_LBUTTON) < 0);
        }

        private bool IsMouseButtonsSwitched()
        {
            return (GetSystemMetrics(SYSTEM_METRICS.SM_SWAPBUTTON) != 0);
        }

        private void ResizeMask()
        {
            RECT tr = new RECT();
            SCROLLBARINFO sb = new SCROLLBARINFO();
            sb.cbSize = Marshal.SizeOf(sb);
            IntPtr hParent = GetParent(_hControlWnd);
            Point pt = new Point();

            if (HasVertical())
            {
                GetScrollBarInfo(_hControlWnd, OBJID_VSCROLL, ref sb);
                tr = sb.rcScrollBar;
                pt.X = tr.Left;
                pt.Y = tr.Top;
                ScreenToClient(hParent, ref pt);
                SetWindowPos(_hVerticalMaskWnd, IntPtr.Zero, pt.X, pt.Y, tr.Right - tr.Left, tr.Bottom - tr.Top, SWP_NOACTIVATE | SWP_NOOWNERZORDER | SWP_NOZORDER | SWP_SHOWWINDOW);
            }
            if (HasHorizontal())
            {
                GetScrollBarInfo(_hControlWnd, OBJID_HSCROLL, ref sb);
                tr = sb.rcScrollBar;
                pt.X = tr.Left;
                pt.Y = tr.Top;
                ScreenToClient(hParent, ref pt);
                SetWindowPos(_hHorizontalMaskWnd, IntPtr.Zero, pt.X, pt.Y, tr.Right - tr.Left, tr.Bottom - tr.Top, SWP_NOACTIVATE | SWP_NOOWNERZORDER | SWP_NOZORDER | SWP_SHOWWINDOW);
            }
            if (HasSizer())
            {
                GetScrollBarInfo(_hControlWnd, OBJID_HSCROLL, ref sb);
                tr = new RECT(sb.rcScrollBar.Right, sb.rcScrollBar.Top, sb.rcScrollBar.Right + _iArrowCx, sb.rcScrollBar.Bottom);
                pt.X = tr.Left;
                pt.Y = tr.Top;
                ScreenToClient(hParent, ref pt);
                SetWindowPos(_hSizerMaskWnd, IntPtr.Zero, pt.X, pt.Y, tr.Right - tr.Left, tr.Bottom - tr.Top, SWP_NOACTIVATE | SWP_NOOWNERZORDER | SWP_NOZORDER | SWP_SHOWWINDOW);
            }
        }

        private void ScrollbarMetrics()
        {
            _iArrowCx = GetSystemMetrics(SYSTEM_METRICS.SM_CXVSCROLL);
            _iArrowCy = GetSystemMetrics(SYSTEM_METRICS.SM_CYVSCROLL);
        }

        public void Dispose()
        {
            try
            {
                this.ReleaseHandle();
                if (_hVerticalMaskWnd != IntPtr.Zero) DestroyWindow(_hVerticalMaskWnd);
                if (_hHorizontalMaskWnd != IntPtr.Zero) DestroyWindow(_hHorizontalMaskWnd);
                if (_hSizerMaskWnd != IntPtr.Zero) DestroyWindow(_hSizerMaskWnd);
            }
            catch { }
            GC.SuppressFinalize(this);
        }
        #endregion

        #region WndProc
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_PAINT:
                case WM_NCPAINT:
                    DrawScrollBar();
                    base.WndProc(ref m);
                    break;

                case WM_HSCROLL:
                case WM_VSCROLL:
                case WM_NCMOUSEMOVE:
                case WM_MOUSEWHEEL:
                    _bTrackingMouse = true;
                    DrawScrollBar();
                    base.WndProc(ref m);
                    break;

                case WM_MOUSEMOVE:
                    if (_bTrackingMouse)
                        _bTrackingMouse = false;
                    if (_isFocus == true)
                    {
                        _isFocus = false;
                        DrawScrollBar();
                    }
                    base.WndProc(ref m);
                    break;

                case WM_SIZE:
                case WM_MOVE:
                    ResizeMask();
                    base.WndProc(ref m);
                    break;

                default:
                    CheckBarState();
                    base.WndProc(ref m);
                    break;
            }
        }
        #endregion
    }

    public class ColorSccrollBarSkin : ScrollBarSkin
    {
        public Color BackgroundColor { get; set; }
        public Color ScrollBarColor { get; set; }
        public Color HighlightScrollBarColor { get; set; }
        public Color ArrowColor { get; set; }

        public ColorSccrollBarSkin(IntPtr hWnd) : base(hWnd)
        {
            BackgroundColor = Color.FromArgb(0x3E, 0x3E, 0x3E);
            ScrollBarColor = Color.FromArgb(0x68, 0x68, 0x68);
            HighlightScrollBarColor = Color.FromArgb(168, 168, 168);
            ArrowColor = Color.FromArgb(0x99, 0x99, 0x99);

        }

        protected override void DrawVerticalScrollBar(Graphics g, int scrollBarWidth, int scrollBarHeight, int thumbTop, int thumbBottom, int arrowSize, bool isMouseOver, bool isClick)
        {
            Color thumbColor = (isMouseOver == true || isClick == true) ? HighlightScrollBarColor : ScrollBarColor;
            int margin = scrollBarWidth > 12 ? 4 : 0;
            int realArrowSize = arrowSize - margin * 2;
            float offset = realArrowSize / 3.0f;


            using (Brush bgBrush = new SolidBrush(BackgroundColor))
            using (Brush brush = new SolidBrush(thumbColor))
            using (Brush arrowBrush = new SolidBrush(ArrowColor))
            {
                g.FillRectangle(bgBrush, new Rectangle(0, 0, scrollBarWidth, scrollBarHeight));
                g.FillRectangle(brush, new Rectangle(margin, thumbTop, scrollBarWidth - margin * 2, thumbBottom - thumbTop));
                FillPath(g, new PointF(margin-1, margin + realArrowSize - offset), 
                    new PointF(margin + realArrowSize, margin + realArrowSize - offset), new PointF(margin + realArrowSize/2.0f - 0.5f, margin), arrowBrush);
                FillPath(g, new PointF(margin - 1, scrollBarHeight - arrowSize + margin + offset),
                    new PointF(margin + realArrowSize, scrollBarHeight - arrowSize + margin + offset), new PointF(margin + realArrowSize / 2.0f - 0.5f, scrollBarHeight - margin), arrowBrush);
            }
        }

        protected void FillPath(Graphics g, PointF p0, PointF p1, PointF p2, Brush brush)
        {
            SmoothingMode oldMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            GraphicsPath path = new GraphicsPath();
            path.AddLine(p0, p1);
            path.AddLine(p1, p2);
            path.AddLine(p2, p0);
            g.FillPath(brush, path);
            g.SmoothingMode = oldMode;
        }

        protected override void DrawSizer(Graphics g, int sizerWidth, int sizerHeight)
        {
        }

        protected override void DrawHorizontalScrollBar(Graphics g, int scrollBarWidth, int scrollBarHeight, int thumbTop, int thumbBottom, int arrowSize, bool isMouseOver, bool isClick)
        {
        }
    }
}
