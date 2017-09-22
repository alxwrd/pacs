using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ppaocr
{
    class MySendInput
    {
        public enum VK : ushort
        {
            SHIFT = 0x10,
            CONTROL = 0x11,
            MENU = 0x12,
            ESCAPE = 0x1B,
            BACK = 0x08,
            TAB = 0x09,
            RETURN = 0x0D,
            PRIOR = 0x21,
            NEXT = 0x22,
            END = 0x23,
            HOME = 0x24,
            LEFT = 0x25,
            UP = 0x26,
            RIGHT = 0x27,
            DOWN = 0x28,
            SELECT = 0x29,
            PRINT = 0x2A,
            EXECUTE = 0x2B,
            SNAPSHOT = 0x2C,
            INSERT = 0x2D,
            DELETE = 0x2E,
            HELP = 0x2F,
            VK_0 = 0x30,
            VK_1 = 0x31,
            VK_2 = 0x32,
            VK_3 = 0x33,
            VK_4 = 0x34,
            VK_5 = 0x35,
            VK_6 = 0x36,
            VK_7 = 0x37,
            VK_8 = 0x38,
            VK_9 = 0x39,
            VK_A = 0x41,
            VK_B = 0x42,
            VK_C = 0x43,
            VK_W = 0x57,
            VK_Z = 0x5A,
            NUMPAD0 = 0x60,
            NUMPAD1 = 0x61,
            NUMPAD2 = 0x62,
            NUMPAD3 = 0x63,
            NUMPAD4 = 0x64,
            NUMPAD5 = 0x65,
            NUMPAD6 = 0x66,
            NUMPAD7 = 0x67,
            NUMPAD8 = 0x68,
            NUMPAD9 = 0x69,
            MULTIPLY = 0x6A,
            ADD = 0x6B,
            SEPARATOR = 0x6C,
            SUBTRACT = 0x6D,
            DECIMAL = 0x6E,
            DIVIDE = 0x6F,
            F1 = 0x70,
            F2 = 0x71,
            F3 = 0x72,
            F4 = 0x73,
            F5 = 0x74,
            F6 = 0x75,
            F7 = 0x76,
            F8 = 0x77,
            F9 = 0x78,
            F10 = 0x79,
            F11 = 0x7A,
            F12 = 0x7B,
            VK_LCONTROL = 0xA2,
            OEM_1 = 0xBA,   // ',:' for US
            OEM_PLUS = 0xBB,   // '+' any country
            OEM_COMMA = 0xBC,   // ',' any country
            OEM_MINUS = 0xBD,   // '-' any country
            OEM_PERIOD = 0xBE,   // '.' any country
            OEM_2 = 0xBF,   // '/?' for US
            OEM_3 = 0xC0,   // '`~' for US
            MEDIA_NEXT_TRACK = 0xB0,
            MEDIA_PREV_TRACK = 0xB1,
            MEDIA_STOP = 0xB2,
            MEDIA_PLAY_PAUSE = 0xB3,
            LWIN = 0x5B,
            RWIN = 0x5C
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HARDWAREINPUT
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }

        [StructLayout(LayoutKind.Explicit)]
        struct INPUT
        {
            [FieldOffset(0)]
            public int type;
            [FieldOffset(4)] //*
            public MOUSEINPUT mi;
            [FieldOffset(4)] //*
            public KEYBDINPUT ki;
            [FieldOffset(4)] //*
            public HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Explicit)]
        struct INPUT64
        {
            [FieldOffset(0)]
            public int type;
            [FieldOffset(8)] //*
            public MOUSEINPUT mi;
            [FieldOffset(8)] //*
            public KEYBDINPUT ki;
            [FieldOffset(8)] //*
            public HARDWAREINPUT hi;
        }


        public enum KEYEVENT : uint
        {
            KEYDOWN = 0,
            EXTENDEDKEY = 1,
            KEYUP = 2,
            UNICODE = 4,
            SCANCODE = 8
        }

        private void AbsolutePixelToMickeys(ref int x, ref int y)
        {
            // Convert x,y to mouse units used for SendInput, use all screens so that
            // multi-monitor configurations are supported.
            int xMin=0, xMax=0, yMin=0, yMax=0;
            Screen screenForPoint = null;
            foreach (Screen s in Screen.AllScreens)
            {
                if (s.Bounds.Left < xMin)
                    xMin = s.Bounds.Left;
                if (s.Bounds.Right > xMax)
                    xMax = s.Bounds.Right;
                if (s.Bounds.Top < yMin)
                    yMin = s.Bounds.Top;
                if (s.Bounds.Bottom > yMax)
                    yMax = s.Bounds.Bottom;
                if (x >= s.Bounds.Left && x < s.Bounds.Right && y >= s.Bounds.Top && y < s.Bounds.Bottom)
                    screenForPoint = s;
            }

            if (screenForPoint == null)
            {
                throw new Exception("Coordinates not within screen: " + x.ToString() + ", " + y.ToString());
            }

            x = 65535 * (x - xMin) / (xMax - xMin-1);
            y = 65535 * (y - yMin) / (yMax - yMin-1);
        }

        public void SendMouseClick(int x, int y)
        {
            // x,y are absolute coordinates in units of pixels

            // SendInput needs normalized (0-65535) absolute coordinates
            // so normalize coordinates first
            AbsolutePixelToMickeys(ref x, ref y);

            if (IntPtr.Size < 8)
            {
                INPUT[] structInputs = new INPUT[2];
                structInputs[0].type = INPUT_MOUSE;

                structInputs[0].mi.dx = x;
                structInputs[0].mi.dy = y;
                structInputs[0].mi.mouseData = 0;
                structInputs[0].mi.time = 0;
                structInputs[0].mi.dwExtraInfo = IntPtr.Zero;
                structInputs[0].mi.dwFlags = MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE | MOUSEEVENTF_VIRTUALDESK | MOUSEEVENTF_LEFTDOWN;

                structInputs[1] = structInputs[0];
                structInputs[1].mi.dwFlags = MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE | MOUSEEVENTF_VIRTUALDESK | MOUSEEVENTF_LEFTUP;

                SendInput(2, structInputs, Marshal.SizeOf(typeof(INPUT)));
            }
            else
            {
                INPUT64[] structInputs = new INPUT64[2];
                structInputs[0].type = INPUT_MOUSE;

                structInputs[0].mi.dx = x;
                structInputs[0].mi.dy = y;
                structInputs[0].mi.mouseData = 0;
                structInputs[0].mi.time = 0;
                structInputs[0].mi.dwExtraInfo = IntPtr.Zero;
                structInputs[0].mi.dwFlags = MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE | MOUSEEVENTF_VIRTUALDESK | MOUSEEVENTF_LEFTDOWN;

                structInputs[1] = structInputs[0];
                structInputs[1].mi.dwFlags = MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE | MOUSEEVENTF_VIRTUALDESK | MOUSEEVENTF_LEFTUP;

                SendInput64(2, structInputs, Marshal.SizeOf(typeof(INPUT64)));

            }

        }

        public static int MakeLParam(int x, int y)
        {
            return (y << 16) + x;
        }

        public void SendMouseClick(IntPtr hWnd, int x, int y)
        {
            //x = 65535 * x / 800;
            //y = 65535 * y / 600;

            int lParam = (y << 16) + x;
            int result = SendMessage(hWnd, WM_LBUTTONDOWN, MK_LBUTTON, lParam);
            System.Threading.Thread.Sleep(100);
            result = SendMessage(hWnd, WM_LBUTTONUP, 0, lParam);
        }

        public void DoKeyboard(VK key, KEYEVENT keyEvent)
        {

            if (IntPtr.Size < 8)
            {
                INPUT[] structInputs = new INPUT[1];
                structInputs[0].type = INPUT_KEYBOARD;

                structInputs[0].ki.wScan = 0;
                structInputs[0].ki.time = 0;
                structInputs[0].ki.dwFlags = (uint)keyEvent;
                structInputs[0].ki.dwExtraInfo = IntPtr.Zero;
                structInputs[0].ki.wVk = (ushort)key;
                SendInput(1, structInputs, Marshal.SizeOf(typeof(INPUT)));
            }
            else
            {
                INPUT64[] structInputs = new INPUT64[1];
                structInputs[0].type = INPUT_KEYBOARD;

                structInputs[0].ki.wScan = 0;
                structInputs[0].ki.time = 0;
                structInputs[0].ki.dwFlags = (uint)keyEvent;
                structInputs[0].ki.dwExtraInfo = IntPtr.Zero;
                structInputs[0].ki.wVk = (ushort)key;
                SendInput64(1, structInputs, Marshal.SizeOf(typeof(INPUT64)));

            }
        }

        [DllImport("user32.dll")]
        static extern IntPtr GetMessageExtraInfo();

        const int INPUT_MOUSE = 0;
        const int INPUT_KEYBOARD = 1;
        const int INPUT_HARDWARE = 2;
        const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        const uint KEYEVENTF_KEYUP = 0x0002;
        const uint KEYEVENTF_UNICODE = 0x0004;
        const uint KEYEVENTF_SCANCODE = 0x0008;
        const uint XBUTTON1 = 0x0001;
        const uint XBUTTON2 = 0x0002;
        const uint MOUSEEVENTF_MOVE = 0x0001;
        const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        const uint MOUSEEVENTF_LEFTUP = 0x0004;
        const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
        const uint MOUSEEVENTF_RIGHTUP = 0x0010;
        const uint MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        const uint MOUSEEVENTF_MIDDLEUP = 0x0040;
        const uint MOUSEEVENTF_XDOWN = 0x0080;
        const uint MOUSEEVENTF_XUP = 0x0100;
        const uint MOUSEEVENTF_WHEEL = 0x0800;
        const uint MOUSEEVENTF_VIRTUALDESK = 0x4000;
        const uint MOUSEEVENTF_ABSOLUTE = 0x8000;

        [DllImport("user32.dll", SetLastError=true)]
        static extern uint SendInput(uint nInputs, INPUT [] pInputs, int cbSize);

        [DllImport("user32.dll", EntryPoint="SendInput")]
        static extern uint SendInput64(uint nInputs, INPUT64[] pInputs, int cbSize);

        public enum WMessages : uint
        { 
            WM_SETCURSOR = 0x20,
            WM_KEYDOWN = 0x100,  //Key down 
            WM_KEYUP = 0x101,   //Key up 
            WM_LBUTTONDOWN = 0x201, //Left mousebutton down 
            WM_LBUTTONUP = 0x202,  //Left mousebutton up 
            WM_LBUTTONDBLCLK = 0x203, //Left mousebutton doubleclick 
            WM_RBUTTONDOWN = 0x204, //Right mousebutton down 
            WM_RBUTTONUP = 0x205,   //Right mousebutton up 
            WM_RBUTTONDBLCLK = 0x206, //Right mousebutton doubleclick 
        } 

        public const uint WM_LBUTTONDOWN   = 0x0201;
        public const uint WM_LBUTTONUP = 0x0202;
        public const uint WM_MOUSEMOVE = 0x0200;

        public const int  MK_LBUTTON = 0x1;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd,      // handle to destination window
                  uint Msg,       // message
                  int wParam,  // first message parameter
                  int lParam   // second message parameter
                  );

    }

}
