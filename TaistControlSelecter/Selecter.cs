using System;
using System.Threading;
using System.Runtime.InteropServices;
using System.Reflection;

using Mock.Nature.Native;
using Mock.Tools.Controls;

namespace TaistControlSelecter
{
    internal delegate void SelectEventHandler(int x, int y);
    internal delegate void EventHandler();
    internal class Selecter
    {
        public event EventHandler VisualEvent = null;
        public event EventHandler HiddenEvent = null;
        public event EventHandler MouseMoveEvent = null;
        private POINT lastPoint;

        private Thread t = null;
        private bool state = false;

        private bool visual = false;

        internal Selecter()
        {
        }

        internal void Start()
        {
            state = true;
            t = new Thread(CheckMouseAndKeybordSwitch);

            t.IsBackground = true;
            t.Start();
        }

        private void CheckMouseAndKeybordSwitch()
        {
            while (state)
            {
                if (IsCtrlPressed)
                {
                    if (!visual)
                    {
                        visual = true;
                        VisualEvent();
                    }
                }
                else
                {
                    if (visual)
                    {
                        visual = false;
                        HiddenEvent();
                    }
                }
                POINT p = Mouse.Position;
                if (lastPoint.x == p.x && lastPoint.y == p.y)
                {
                }
                else
                {
                    lastPoint = p;
                    MouseMoveEvent();
                }
                Thread.Sleep(19);
            }
        }

        private bool IsCtrlPressed
        {
            get
            {
                return (Mock.Nature.Native.NativeMethods.GetAsyncKeyState(17) & 0x8000) != 0;
            }
        }

        //private int MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        //{
        //    if (nCode == HC_ACTION)
        //    {
        //        MouseHookStruct MouseInfo = (MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct));
        //        if ((int)wParam == WM_LBUTTONUP && controlFlag)
        //        {
        //            SelectEvent(MouseInfo.pt.x, MouseInfo.pt.y);
        //            return 1;
        //        }
        //        else if ((int)wParam == WM_LBUTTONDOWN && controlFlag)
        //        {
        //            return 1;
        //        }
        //        else if ((int)wParam == WM_RBUTTONDOWN && controlFlag)
        //        {
        //            return 1;
        //        }
        //        else if ((int)wParam == WM_RBUTTONUP && controlFlag)
        //        {
        //            if (CloseEvent != null)
        //            {
        //                CloseEvent();
        //            }
        //            return 1;
        //        }
        //        else if ((int)wParam == WM_MOUSEMOVE)
        //        {
        //            if (MouseMoveEvent != null)
        //            {
        //                MouseMoveEvent(MouseInfo.pt.x, MouseInfo.pt.y);
        //            }
        //            return 0;
        //        }
        //    }
        //    return CallNextHookEx(hMouseHook, nCode, wParam, lParam);
        //}

        //private int KeybordHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        //{
        //    if (nCode == HC_ACTION)
        //    {
        //        KeybordHookStruct KeybordInfo = (KeybordHookStruct)Marshal.PtrToStructure(lParam, typeof(KeybordHookStruct));

        //        if (SelectEvent != null && (int)wParam == WM_KEYDOWN && KeybordInfo.vkCode == 162)
        //        {
        //            controlFlag = true;
        //        }
        //        else if (SelectEvent != null && (int)wParam == WM_KEYUP && KeybordInfo.vkCode == 162)
        //        {
        //            controlFlag = false;
        //        }
        //    }
        //    return CallNextHookEx(hKeybordHook, nCode, wParam, lParam);
        //}

        internal void Close()
        {
            state = false;
        }

    }
}
