namespace Mock.Nature.Native
{
    using System;
    using System.Runtime.InteropServices;
    public struct RECT
    {
        public uint Left;
        public uint Top;
        public uint Right;
        public uint Bottom;
    }

    public struct POINT
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential, Size = 40)]
    public struct PROCESS_MEMORY_COUNTERS
    {
        public UInt32 cbSize;
        public UInt32 PageFaultCount;
        public UInt32 PeakWorkingSetSize;
        public UInt32 WorkingSetSize;
        public UInt32 QuotaPeakPagedPoolUsage;
        public UInt32 QuotaPagedPoolUsage;
        public UInt32 QuotaPeakNonPagedPoolUsage;
        public UInt32 QuotaNonPagedPoolUsage;
        public UInt32 PagefileUsage;
        public UInt32 PeakPagefileUsage;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct INTERNET_CACHE_ENTRY_INFO
    {
        public int dwStructSize;
        public IntPtr lpszSourceUrlName;
        public IntPtr lpszLocalFileName;
        public int CacheEntryType;
        public int dwUseCount;
        public int dwHitRate;
        public int dwSizeLow;
        public int dwSizeHigh;
        public System.Runtime.InteropServices.ComTypes.FILETIME LastModifiedTime;
        public System.Runtime.InteropServices.ComTypes.FILETIME ExpireTime;
        public System.Runtime.InteropServices.ComTypes.FILETIME LastAccessTime;
        public System.Runtime.InteropServices.ComTypes.FILETIME LastSyncTime;
        public IntPtr lpHeaderInfo;
        public int dwHeaderInfoSize;
        public IntPtr lpszFileExtension;
        public int dwExemptDelta;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct SYSTEMTIME
    {
        public short wYear;
        public short wMonth;
        public short wDayOfWeek;
        public short wDay;
        public short wHour;
        public short wMinute;
        public short wSecond;
        public short wMilliseconds;
    }

    public class SetWindowPosParameters
    {
        /// <summary>
        /// 将窗口置于Z序的底部。如果参数hWnd标识了一个顶层窗口，则窗口失去顶级位置，并且被置在其他窗口的底部。
        /// </summary>
        public static readonly int HWND_BOTTOM = 1;

        /// <summary>
        /// 将窗口置于所有非顶层窗口之上（即在所有顶层窗口之后）。如果窗口已经是非顶层窗口则该标志不起作用。
        /// </summary>
        public static readonly int HWND_DOTTOPMOST = -2;

        /// <summary>
        /// 将窗口置于Z序的顶部。
        /// </summary>
        public static readonly int HWND_TOP = 0;

        /// <summary>
        /// 将窗口置于所有非顶层窗口之上。即使窗口未被激活窗口也将保持顶级位置。
        /// </summary>
        public static readonly int HWND_TOPMOST = -1;

        /// <summary>
        /// 如果调用进程不拥有窗口，系统会向拥有窗口的线程发出需求。这就防止调用线程在其他线程处理需求的时候发生死锁。
        /// </summary>
        public static readonly int SWP_ASNCWINDOWPOS = 0x4000;

        /// <summary>
        /// 防止产生WM_SYNCPAINT消息。
        /// </summary>
        public static readonly int SWP_DEFERERASE = 0x2000;

        /// <summary>
        /// 在窗口周围画一个边框（定义在窗口类描述中）。
        /// </summary>
        public static readonly int SWP_DRAWFRAME = 0x0020;

        /// <summary>
        /// 给窗口发送WM_NCCALCSIZE消息，即使窗口尺寸没有改变也会发送该消息。如果未指定这个标志，只有在改变了窗口尺寸时才发送WM_NCCALCSIZE。
        /// </summary>
        public static readonly int SWP_FRAMECHANGED = 0x0020;

        /// <summary>
        /// 隐藏窗口。
        /// </summary>
        public static readonly int SWP_HIDEWINDOW = 0x0080;

        /// <summary>
        /// 不激活窗口。如果未设置标志，则窗口被激活，并被设置到其他最高级窗口或非最高级组的顶部（根据参数hWndlnsertAfter设置）。
        /// </summary>
        public static readonly int SWP_NOACTIVATE = 0x0010;

        /// <summary>
        /// 清除客户区的所有内容。如果未设置该标志，客户区的有效内容被保存并且在窗口尺寸更新和重定位后拷贝回客户区。
        /// </summary>
        public static readonly int SWP_NOCOPYBITS = 0x0100;

        /// <summary>
        /// 维持当前位置（忽略X和Y参数）。
        /// </summary>
        public static readonly int SWP_NOMOVE = 0x0002;

        /// <summary>
        /// 不改变z序中的所有者窗口的位置。
        /// </summary>
        public static readonly int SWP_NOOWNERZORDER = 0x0200;

        /// <summary>
        /// 不重画改变的内容。如果设置了这个标志，则不发生任何重画动作。适用于客户区和非客户区（包括标题栏和滚动条）和任何由于窗回移动而露出的父窗口的所有部分。如果设置了这个标志，应用程序必须明确地使窗口无效并区重画窗口的任何部分和父窗口需要重画的部分。
        /// </summary>
        public static readonly int SWP_NOREDRAW = 0x0008;

        /// <summary>
        /// 与SWP_NOOWNERZORDER标志相同。
        /// </summary>
        public static readonly int SWP_NOREPOSITION = 0x0200;

        /// <summary>
        /// 防止窗口接收WM_WINDOWPOSCHANGING消息。
        /// </summary>
        public static readonly int SWP_NOSENDCHANGING = 0x0400;

        /// <summary>
        /// 维持当前尺寸（忽略cx和Cy参数）。
        /// </summary>
        public static readonly int SWP_NOSIZE = 0x0001;

        /// <summary>
        /// 维持当前Z序（忽略hWndlnsertAfter参数）。
        /// </summary>
        public static readonly int SWP_NOZORDER = 0x0004;

        /// <summary>
        /// 显示窗口。
        /// </summary>
        public static readonly int SWP_SHOWWINDOW = 0x0040;
    }
}
