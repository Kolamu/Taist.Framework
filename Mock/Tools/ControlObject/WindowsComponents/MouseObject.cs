namespace Mock.Tools.Controls
{
    using System.Runtime.InteropServices;
    using Mock.Tools.Exception;
    using Mock.Nature.Native;
    internal class MouseObject : WinObject
    {
        internal MouseObject()
        {
        }
        /// <summary>
        /// Mouse click event
        /// </summary>
        /// <param name="mouseEventFlag">MouseEventFlag </param>
        /// <param name="incrementX">X coordinate</param>
        /// <param name="incrementY">Y coordinate</param>
        /// <param name="data"></param>
        /// <param name="extraInfo"></param>
        [DllImport("user32.dll")]
        private extern static void mouse_event(int mouseEventFlag, int incrementX, int incrementY, int data, int extraInfo);

        /// <summary>
        /// Set Mouse Cursor Position
        /// </summary>
        /// <param name="x">X Coordinary</param>
        /// <param name="y">Y Coordinary</param>
        [DllImport("user32.dll", SetLastError=true)]
        private extern static bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        private extern static bool GetCursorPos(ref POINT p);

        private const int MOUSEEVENTF_MOVE = 0x0001;
        private const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const int MOUSEEVENTF_LEFTUP = 0x0004;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const int MOUSEEVENTF_RIGHTUP = 0x0010;
        private const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        private const int MOUSEEVENTF_MIDDLEUP = 0x0040;
        private const int MOUSEEVENTF_ABSOLUTE = 0x8000;
        private const int MOUSEEVENTF_WHEEL = 0x0800;

        private int x;
        private int y;

        internal MouseObject(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        internal void Click()
        {
            //NativeMethods.BlockInput(true);
            POINT p = new POINT();
            GetCursorPos(ref p);
            //SetCursorPos(x, y);
            mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE, x * 65536 / System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width, y * 65536 / System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height, 0, 0);
            mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, x * 65536 / System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width, y * 65536 / System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height, 0, 0);
            //Wait(20);
            //mouse_event(, x, y, 0, 0);
            //Wait(20);
            SetCursorPos(p.x, p.y);
            //while (!NativeMethods.BlockInput(false))
            //{
            //    Wait(100);
            //    LogManager.Error("Input Unlock Fail");
            //}
        }

        internal void Roll(bool up)
        {
            //NativeMethods.BlockInput(true);
            POINT p = new POINT();
            GetCursorPos(ref p);
            Wait(20);
            if (up)
            {
                mouse_event(MOUSEEVENTF_WHEEL, p.x, p.y, 10, 0);
            }
            else
            {
                mouse_event(MOUSEEVENTF_WHEEL, p.x, p.y, -10, 0);
            }
            Wait(20);
            //while (!NativeMethods.BlockInput(false))
            //{
            //    Wait(100);
            //    LogManager.Error("Input Unlock Fail");
            //}
        }

        internal void RightClick()
        {
            //NativeMethods.BlockInput(true);
            POINT p = new POINT();
            GetCursorPos(ref p);
            //SetCursorPos(x, y);
            //Wait(20);
            mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE, x * 65536 / System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width, y * 65536 / System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height, 0, 0);
            mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, x * 65536 / System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width, y * 65536 / System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height, 0, 0);

            //mouse_event(, x, y, 0, 0);
            //Wait(20);
            SetCursorPos(p.x, p.y);
            //while (!NativeMethods.BlockInput(false))
            //{
            //    Wait(100);
            //    LogManager.Error("Input Unlock Fail");
            //}
        }

        internal void DbClick()
        {
            //NativeMethods.BlockInput(true);
            POINT p = new POINT();
            GetCursorPos(ref p);
            //SetCursorPos(x, y);
            //Wait(20);
            //mouse_event(MOUSEEVENTF_LEFTDOWN, x, y, 0, 0);
            //Wait(20);
            //mouse_event(MOUSEEVENTF_LEFTUP, x, y, 0, 0);
            //Wait(10);
            //mouse_event(MOUSEEVENTF_LEFTDOWN, x, y, 0, 0);
            //Wait(20);
            //mouse_event(MOUSEEVENTF_LEFTUP, x, y, 0, 0);
            //Wait(20);
            mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE, x * 65536 / System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width, y * 65536 / System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height, 0, 0);
            mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, x * 65536 / System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width, y * 65536 / System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height, 0, 0);
            mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, x * 65536 / System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width, y * 65536 / System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height, 0, 0);

            SetCursorPos(p.x, p.y);
            //while (!NativeMethods.BlockInput(false))
            //{
            //    Wait(100);
            //    LogManager.Error("Input Unlock Fail");
            //}
        }

        internal void Move()
        {
            //NativeMethods.BlockInput(true);
            SetCursorPos(x, y);
            //while (!NativeMethods.BlockInput(false))
            //{
            //    Wait(100);
            //    Robot.Speak("Input Unlock Fail");
            //}
        }

        internal POINT Position
        {
            get
            {
                POINT pt = new POINT();
                GetCursorPos(ref pt);
                return pt;
            }
            set
            {
                SetCursorPos(value.x, value.y);
            }
        }
    }
}
