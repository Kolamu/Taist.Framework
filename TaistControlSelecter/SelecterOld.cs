using System;
using System.Runtime.InteropServices;
using System.Reflection;

namespace TaistControlSelecter
{
    //internal delegate void SelectEventHandler(int x, int y);
    //internal delegate void EventHandler();
    internal class SelecterOld
    {
        private int hMouseHook = 0;
        private int hKeybordHook = 0;
        private static HookProc MouseHook;
        private static HookProc KeybordHook;
        private bool controlFlag = false;

        public SelectEventHandler SelectEvent = null;
        public EventHandler CloseEvent = null;
        public SelectEventHandler MouseMoveEvent = null;

        internal SelecterOld()
        {
            MouseHook = MouseHookProc;
            KeybordHook = KeybordHookProc;
            hMouseHook = SetWindowsHookEx(WH_MOUSE_LL, MouseHook, System.Diagnostics.Process.GetCurrentProcess().MainModule.BaseAddress, 0);
            hKeybordHook = SetWindowsHookEx(WH_KEYBORD_LL, KeybordHook, System.Diagnostics.Process.GetCurrentProcess().MainModule.BaseAddress, 0);
            if (hMouseHook == 0 || hKeybordHook == 0)
            {
                throw new Exception("Hook Fail");
            }
            
        }

        private int MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode == HC_ACTION)
            {
                MouseHookStruct MouseInfo = (MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct));
                if ((int)wParam == WM_LBUTTONUP && controlFlag)
                {
                    SelectEvent(MouseInfo.pt.x, MouseInfo.pt.y);
                    return 1;
                }
                else if ((int)wParam == WM_LBUTTONDOWN && controlFlag)
                {
                    return 1;
                }
                else if ((int)wParam == WM_RBUTTONDOWN && controlFlag)
                {
                    return 1;
                }
                else if ((int)wParam == WM_RBUTTONUP && controlFlag)
                {
                    if (CloseEvent != null)
                    {
                        CloseEvent();
                    }
                    return 1;
                }
                else if ((int)wParam == WM_MOUSEMOVE)
                {
                    if (MouseMoveEvent != null)
                    {
                        MouseMoveEvent(MouseInfo.pt.x, MouseInfo.pt.y);
                    }
                    return 0;
                }
            }
            return CallNextHookEx(hMouseHook, nCode, wParam, lParam);
        }

        private int KeybordHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode == HC_ACTION)
            {
                KeybordHookStruct KeybordInfo = (KeybordHookStruct)Marshal.PtrToStructure(lParam, typeof(KeybordHookStruct));

                if (SelectEvent != null && (int)wParam == WM_KEYDOWN && KeybordInfo.vkCode == 162)
                {
                    controlFlag = true;
                }
                else if (SelectEvent != null && (int)wParam == WM_KEYUP && KeybordInfo.vkCode == 162)
                {
                    controlFlag = false;
                }
            }
            return CallNextHookEx(hKeybordHook, nCode, wParam, lParam);
        }

        internal void Close()
        {
            UnhookWindowsHookEx(hKeybordHook);
            UnhookWindowsHookEx(hMouseHook);
        }

        #region Hook
        private readonly int WH_KEYBORD_LL = 13;
        private readonly int WH_MOUSE_LL = 14;
        private readonly int HC_ACTION = 0;
        //private readonly int WM_MOUSEFIRST = 0x0200;
        private readonly int WM_MOUSEMOVE = 0x0200;
        private readonly int WM_LBUTTONDOWN = 0x0201;
        private readonly int WM_LBUTTONUP = 0x0202;
        //private readonly int WM_LBUTTONDBLCLK = 0x0203;
        private readonly int WM_RBUTTONDOWN = 0x0204;
        private readonly int WM_RBUTTONUP = 0x0205;
        //private readonly int WM_RBUTTONDBLCLK = 0x0206;
        //private readonly int WM_MBUTTONDOWN = 0x0207;
        //private readonly int WM_MBUTTONUP = 0x0208;
        //private readonly int WM_MBUTTONDBLCLK = 0x0209;
        private readonly int WM_KEYDOWN = 0x0100;
        private readonly int WM_KEYUP = 0x0101;
        //private readonly int WM_CHAR = 0x0102;
        //private readonly int WM_DEADCHAR = 0x0103;
        //private readonly int WM_SYSKEYDOWN = 0x0104;
        //private readonly int WM_SYSKEYUP = 0x0105;
        //private readonly int WM_SYSCHAR = 0x0106;
        //private readonly int WM_SYSDEADCHAR = 0x0107;
        private delegate int HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hINstance, int threadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern bool UnhookWindowsHookEx(int idHook);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int CallNextHookEx(int idHook, int nCode, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        private class POINT
        {
            public int x;
            public int y;
        }
        [StructLayout(LayoutKind.Sequential)]
        private class MouseHookStruct
        {
            public POINT pt;
            public int hWnd;
            public int wHitTestCode;
            public int dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private class KeybordHookStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }
        #endregion

        #region log
        private void Note(string message, string path = "MouseAndKeybordStep.log")
        {
            System.DateTime currentTime = new System.DateTime();
            currentTime = System.DateTime.Now;
            string strT = currentTime.ToString("u");
            string msg = string.Format("[{0,20}] {1}", strT, message);
            if (System.IO.File.Exists(path))
            {
                System.IO.StreamWriter sw = System.IO.File.AppendText(path);
                //sw.WriteLine("[" + strT + "] " + message);
                sw.WriteLine(msg);
                sw.Flush();
                sw.Close();
                sw.Dispose();
            }
            else
            {
                System.IO.File.Delete(path);
                System.IO.StreamWriter sw = new System.IO.StreamWriter(path);
                //sw.WriteLine("[" + strT + "] " + message);
                sw.WriteLine(msg);
                sw.Flush();
                sw.Close();
                sw.Dispose();
            }
        }
        #endregion
    }
}
