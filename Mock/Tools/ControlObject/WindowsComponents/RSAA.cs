namespace Mock.Tools.Controls
{
    using System;
    using Accessibility;
    using System.Threading;
    //using System.Windows.Automation;
    using System.Collections.Generic;

    using Mock.Tools.Exception;
    using Mock.Nature.Native;
    public class RSAA
    {
        private static IAccessible cell = null;
        //private static IAccessible child = null;
        private static WindowsUnit element = null;

        private static void Init()
        {
            object obj = null;
            Guid g = new Guid("{618736E0-3C3D-11CF-810C-00AA00389B71}");

            NativeMethods.AccessibleObjectFromWindow(element.Current.NativeWindowHandle, OBJID.CLIENT, ref g, ref obj);
            if (obj == null)
            {
                throw new NullControlException(RobotContext.WindowName, RobotContext.ElementName);
            }

            cell = (IAccessible)obj;
        }

        public static WindowsUnit Cell
        {
            get
            {
                return element;
            }
            set
            {
                element = value;
                Init();
            }
        }

        public static IAccessible GetElement(IntPtr hWnd)
        {
            object obj = null;
            Guid g = new Guid("{618736E0-3C3D-11CF-810C-00AA00389B71}");

            NativeMethods.AccessibleObjectFromWindow((int)hWnd, OBJID.CLIENT, ref g, ref obj);
            if (obj == null)
            {
                throw new NullControlException(hWnd.ToString());
            }

            return (IAccessible)obj;
        }

        public static IAccessible GetWindow(IntPtr hWnd)
        {
            object obj = null;
            Guid g = new Guid("{618736E0-3C3D-11CF-810C-00AA00389B71}");

            NativeMethods.AccessibleObjectFromWindow((int)hWnd, OBJID.WINDOW, ref g, ref obj);
            if (obj == null)
            {
                throw new NullControlException(hWnd.ToString());
            }

            return (IAccessible)obj;
        }

        public static void Find(string condition)
        {
            Object[] childs = new Object[cell.accChildCount];
            int obtain;
            NativeMethods.AccessibleChildren(cell, 0, cell.accChildCount, childs, out obtain);

            List<IAccessible> rowDatas = new List<IAccessible>();
            foreach (Object o in childs)
            {
                IAccessible child = (IAccessible)o;
                if (child.accRole.ToString() == "28")
                {
                    rowDatas.Add(child);
                }
                //Console.WriteLine("{0} {1} {2}", child.accName, child.accChildCount, child.accRole);
            }

            //IAccessible rowData = null;
            //if (rowIndex == 0)
            //{
            //    rowData = rowDatas[rowDatas.Count - 1];
            //}
            //else
            //{
            //    rowData = rowDatas[rowIndex];
            //}

            //IAccessible retData = null;
            //if (columnName == null)
            //{
            //    retData = rowData;
            //}
            //else
            //{
            //    childs = new Object[rowData.accChildCount];
            //    NativeMethods.AccessibleChildren(rowData, 0, rowData.accChildCount, childs, out obtain);

            //    foreach (object o in childs)
            //    {
            //        bool find = true;
            //        IAccessible child = (IAccessible)o;
            //        string[] name = child.accName.ToString().Split(' ');
            //        string[] columnNames = columnName.Split(' ');
            //        try
            //        {
            //            for (int i = 0; i < columnNames.Length; i++)
            //            {
            //                if (!columnNames[i].Equals(name[i]))
            //                {
            //                    find = false;
            //                    break;
            //                }
            //            }
            //            if (find)
            //            {
            //                retData = child;
            //                break;
            //            }
            //        }
            //        catch { }
            //    }
            //}
            //rowData = null;
            //rowDatas = null;
            //childs = null;
            //obj = null;
            //return retData;
        }

        internal static void DoDefaultAction(object args = null)
        {
            uint windowLong = NativeMethods.GetWindowLong((IntPtr)RobotContext.Window.Current.NativeWindowHandle, SetWindowLongOffsets.GWL_STYLE);
            if ((windowLong & (uint)WindowStyles.WS_DISABLED) == (uint)WindowStyles.WS_DISABLED)
            {
                throw new WindowIsInactiveException(RobotContext.WindowName);
            }
            cell.accDoDefaultAction();
        }

        internal static void AsyncDoDefaultAction()
        {
            LogManager.Debug("AsyncDoDefaultAction");
            AsyncAction(DoDefaultAction);
        }

        internal static void SetValue(object value)
        {
            cell.accValue = (string)value;
        }

        internal static void AsyncSetValue(string value)
        {
            LogManager.Debug("AsyncSetValue");
            AsyncAction(SetValue, value);
        }

        private static void AsyncAction(ParameterizedThreadStart handler, object args = null)
        {
            uint windowLong = NativeMethods.GetWindowLong((IntPtr)RobotContext.Window.Current.NativeWindowHandle, SetWindowLongOffsets.GWL_STYLE);
            if ((windowLong & (uint)WindowStyles.WS_DISABLED) == (uint)WindowStyles.WS_DISABLED)
            {
                throw new WindowIsInactiveException(RobotContext.WindowName);
            }

            if (RobotContext.IsWarningWindowExist)
            {
                throw new WarningWindowExistException();
            }
            Thread t = new Thread(handler);
            t.IsBackground = true;
            t.Start(args);
            Timer timer = new Timer(TimerTick, t, 5000, 1000);
            Thread kill = new Thread(KillTimer);
            TimerStruct ts = new TimerStruct();
            ts.timeout = 6000;
            ts.timer = timer;
            kill.IsBackground = true;
            kill.Start(ts);
        }

        private static void TimerTick(object state)
        {
            Thread t = (Thread)state;
            if (t.ThreadState != ThreadState.Stopped)
            {
                try
                {
                    t.Abort();
                }
                catch { }
            }
            Console.WriteLine("kill process");
        }

        private static void KillTimer(object t)
        {
            Console.WriteLine("killTimer");
            TimerStruct ts = (TimerStruct)t;
            Robot.Recess(ts.timeout);
            ts.timer.Dispose();
        }

        private struct TimerStruct
        {
            public int timeout;
            public Timer timer;
        }
    }
}
