namespace Mock.Nature.Native
{
    using System;
    using System.Text;
    using Accessibility;
    using System.Runtime.InteropServices;

    /// <summary>
    /// 本地方法
    /// </summary>
    public class NativeMethods
    {
        [DllImport("User32.dll", SetLastError = true)]
        public extern static bool SetScrollRange(int hWnd, int nBar, int nMinPos, int nMaxPos, bool bRedraw);

        [DllImport("User32.dll", SetLastError = true)]
        public extern static bool SetScrollRange(IntPtr hWnd, int nBar, int nMinPos, int nMaxPos, bool bRedraw);

        /// <summary>
        /// 取消Hook
        /// </summary>
        /// <param name="handle">Hook 句柄</param>
        [DllImport("User32.dll", SetLastError = true)]
        public extern static void UnhookWindowsHookEx(IntPtr handle); //取消Hook的API

        [DllImport("Psapi.dll", SetLastError = true)]
        public static extern bool GetProcessMemoryInfo(IntPtr hProcess, out PROCESS_MEMORY_COUNTERS lpCreationTime, int cbSize);

        /// <summary>
        /// 添加Hook
        /// </summary>
        /// <param name="idHook"></param>
        /// <param name="lpfn"></param>
        /// <param name="hinstance"></param>
        /// <param name="threadID"></param>
        /// <returns></returns>
        [DllImport("User32.dll", SetLastError = true)]
        public extern static IntPtr SetWindowsHookEx(HookType idHook, [MarshalAs(UnmanagedType.FunctionPtr)]HookProc lpfn, IntPtr hinstance, int threadID);  //设置Hook的API

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="code"></param>
        /// <param name="wparam"></param>
        /// <param name="lparam"></param>
        /// <returns></returns>
        [DllImport("User32.dll", SetLastError = true)]
        public extern static IntPtr CallNextHookEx(IntPtr handle, int code, IntPtr wparam, IntPtr lparam); //取得下一个Hook的API

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetCurrentThreadId(); //取得当前线程编号的API

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetLastError();

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder buf, int len);

        /// <summary>
        /// 获取窗口句柄
        /// </summary>
        /// <param name="lpClassName"></param>
        /// <param name="lpWindowName"></param>
        /// <returns></returns>
        [DllImport("User32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        /// <summary>
        /// 获取子窗口句柄
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="index"></param>
        /// <param name="lpClassName"></param>
        /// <param name="lpWindowName"></param>
        /// <returns></returns>
        [DllImport("User32.dll", EntryPoint = "FindWindowEx", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr hWnd, IntPtr index, string lpClassName, string lpWindowName);

        /// <summary>
        /// 生成lParam参数
        /// </summary>
        /// <param name="LoWord">低位</param>
        /// <param name="HiWord">高位</param>
        /// <returns></returns>
        public static uint MakeLParam(uint LoWord, uint HiWord) { return ((HiWord << 16) | (LoWord & 0xffff)); }
        
        /// <summary>
        /// 获取窗口矩形
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="lpRect"></param>
        /// <returns></returns>
        [DllImport("User32", EntryPoint = "GetClientRect", SetLastError = true)]
        public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        /// <summary>
        /// 设置窗口大小/Z序/位置
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="hWndInsertAfter"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int Width, int Height, int flags);

        /// <summary>
        /// 更新窗口
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("User32.dll", EntryPoint = "UpdateWindow", SetLastError = true)]
        public static extern bool UpdateWindow(IntPtr hWnd);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="fShow"></param>
        /// <returns></returns>
        [DllImport("User32.dll", EntryPoint = "ShowOwnedPopups", SetLastError = true)]
        public static extern bool ShowOwnedPopups(IntPtr hWnd, bool fShow);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="fShow"></param>
        /// <returns></returns>
        [DllImport("User32.dll", EntryPoint = "ShowOwnedPopups", SetLastError = true)]
        public static extern bool ShowOwnedPopups(int hWnd, bool fShow);

        #region MSAA
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="id"></param>
        /// <param name="iid"></param>
        /// <param name="ppvObject"></param>
        /// <returns></returns>
        [DllImport("oleacc.dll")]
        public static extern int AccessibleObjectFromWindow(int hwnd, OBJID id, ref Guid iid, [In, Out, MarshalAs(UnmanagedType.IUnknown)] ref object ppvObject);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paccContainer"></param>
        /// <param name="iChildStart"></param>
        /// <param name="cChildren"></param>
        /// <param name="rgvarChildren"></param>
        /// <param name="pcObtained"></param>
        /// <returns></returns>
        [DllImport("oleacc.dll")]
        public static extern uint AccessibleChildren(IAccessible paccContainer, int iChildStart, int cChildren, [Out] object[] rgvarChildren, out int pcObtained);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paccContainer"></param>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("oleacc.dll")]
        public static extern uint WindowFromAccessibleObject(IAccessible paccContainer, out int hWnd);
        #endregion

        /// <summary>
        /// 判断窗口是否挂起
        /// </summary>
        /// <param name="Hwnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool IsHungAppWindow(IntPtr Hwnd);

        /// <summary>
        /// 判断窗口是否挂起
        /// </summary>
        /// <param name="Hwnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool IsHungAppWindow(int Hwnd);

        /// <summary>
        /// 获取前台窗口
        /// </summary>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// 获取前台窗口
        /// </summary>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindow(IntPtr hWnd, GetWindowOffsets id);

        /// <summary>
        /// 获取前台窗口
        /// </summary>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindow(int hWnd, GetWindowOffsets id);

        /// <summary>
        /// 将制定窗口置顶
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        /// 将制定窗口置顶
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetForegroundWindow(int hWnd);

        /// <summary>
        /// 激活指定窗口
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetActiveWindow(IntPtr hWnd);

        /// <summary>
        /// 激活指定窗口
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetActiveWindow(int hWnd);

        /// <summary>
        /// 获取指定窗口的最顶层子窗口
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetTopWindow(IntPtr hWnd);

        /// <summary>
        /// 获取指定窗口的最顶层子窗口
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetTopWindow(int hWnd);

        /// <summary>
        /// 置顶指定窗口
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool BringWindowToTop(IntPtr hWnd);

        /// <summary>
        /// 置顶指定窗口
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool BringWindowToTop(int hWnd);
        
        /// <summary>
        /// 设置窗口显示状态
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="nCmdShow"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        /// <summary>
        /// 判断窗口是否已经最小化
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool IsIconic(IntPtr hWnd);

        /// <summary>
        /// 获取按键当前状态
        /// 高位（是否按下）低位（上次按键状态）
        /// </summary>
        /// <param name="vKey"></param>
        /// <returns></returns>
        [DllImport("User32.dll", SetLastError = true)]
        public static extern int GetAsyncKeyState(int vKey);

        /// <summary>
        /// 获取窗口标题
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="title"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder title, int size);

        /// <summary>
        /// 检查窗口是否可见
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        /// <summary>
        /// 获取窗口标题长度
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        /// <summary>
        /// 显示窗口
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="nCmdShow"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);

        /// <summary>
        /// 枚举窗口
        /// </summary>
        /// <param name="lpfn"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int EnumWindows(EnumWindowsProc lpfn, int lParam);

        /// <summary>
        /// 枚举指定窗口的子窗口
        /// </summary>
        /// <param name="hWndParent"></param>
        /// <param name="lpfn"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int EnumChildWindows(IntPtr hWndParent, EnumChildWindowsProc lpfn, int lParam);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="nIndex"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowLong(IntPtr hWnd, SetWindowLongOffsets nIndex);

        /// <summary>
        /// 获取窗口进程ID
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        [DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int pid);

        /// <summary>
        /// 判断窗口是否有效
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool IsWindow(IntPtr hWnd);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern Int64 WritePrivateProfileString(string section, string key, string val, string filePath);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="def"></param>
        /// <param name="retVal"></param>
        /// <param name="size"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern Int64 GetPrivateProfileString(string section, string key, string def, System.Text.StringBuilder retVal, int size, string filePath);

        /// <summary>
        /// 向指定窗口发送消息
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError=true)]
        public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        /// <summary>
        /// 向指定窗口发送消息
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int SendMessage(IntPtr hWnd, WindowsMessages msg, int wParam, int lParam);

        /// <summary>
        /// 向指定窗口发送消息
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int SendMessage(IntPtr hWnd, WindowsMessages msg, uint wParam, uint lParam);

        /// <summary>
        /// 向指定窗口发送消息
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int SendMessage(int hWnd, int msg, int wParam, int lParam);

        /// <summary>
        /// 向指定窗口发送消息
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int SendMessage(int hWnd, WindowsMessages msg, int wParam, int lParam);

        /// <summary>
        /// 向指定窗口发送消息
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int SendMessage(int hWnd, WindowsMessages msg, uint wParam, uint lParam);

        /// <summary>
        /// 向指定窗口发送消息
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError=true)]
        public static extern int SendNotifyMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        /// <summary>
        /// 向指定窗口发送消息
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int SendNotifyMessage(IntPtr hWnd, WindowsMessages msg, int wParam, int lParam);

        /// <summary>
        /// 向指定窗口发送消息
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int SendNotifyMessage(IntPtr hWnd, WindowsMessages msg, uint wParam, uint lParam);

        /// <summary>
        /// 向指定窗口发送消息
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int SendNotifyMessage(int hWnd, int msg, int wParam, int lParam);

        /// <summary>
        /// 向指定窗口发送消息
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int SendNotifyMessage(int hWnd, WindowsMessages msg, int wParam, int lParam);

        /// <summary>
        /// 向指定窗口发送消息
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int SendNotifyMessage(int hWnd, WindowsMessages msg, uint wParam, uint lParam);

        /// <summary>
        /// 向指定窗口发送消息
        /// </summary>
        /// <param name="hWnd">接收消息的窗口句柄</param>
        /// <param name="msg">消息类型</param>
        /// <param name="wParam">参数高位wParam</param>
        /// <param name="lParam">参数低位lParam</param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError=true)]
        public static extern int PostMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        /// <summary>
        /// 向指定窗口发送消息
        /// </summary>
        /// <param name="hWnd">接收消息的窗口句柄</param>
        /// <param name="msg">消息类型</param>
        /// <param name="wParam">参数高位wParam</param>
        /// <param name="lParam">参数低位lParam</param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int PostMessage(IntPtr hWnd, WindowsMessages msg, int wParam, int lParam);

        /// <summary>
        /// 向指定窗口发送消息
        /// </summary>
        /// <param name="hWnd">接收消息的窗口句柄</param>
        /// <param name="msg">消息类型</param>
        /// <param name="wParam">参数高位wParam</param>
        /// <param name="lParam">参数低位lParam</param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError=true)]
        public static extern int PostMessage(IntPtr hWnd, int msg, int wParam, string lParam);


        /// <summary>
        /// 向指定窗口发送消息
        /// </summary>
        /// <param name="hWnd">接收消息的窗口句柄</param>
        /// <param name="msg">消息类型</param>
        /// <param name="wParam">参数高位wParam</param>
        /// <param name="lParam">参数低位lParam</param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int PostMessage(int hWnd, int msg, int wParam, int lParam);

        /// <summary>
        /// 向指定窗口发送消息
        /// </summary>
        /// <param name="hWnd">接收消息的窗口句柄</param>
        /// <param name="msg">消息类型</param>
        /// <param name="wParam">参数高位wParam</param>
        /// <param name="lParam">参数低位lParam</param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int PostMessage(int hWnd, WindowsMessages msg, int wParam, int lParam);

        /// <summary>
        /// 向指定窗口发送消息
        /// </summary>
        /// <param name="hWnd">接收消息的窗口句柄</param>
        /// <param name="msg">消息类型</param>
        /// <param name="wParam">参数高位wParam</param>
        /// <param name="lParam">参数低位lParam</param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int PostMessage(int hWnd, int msg, int wParam, string lParam);


        /// <summary>
        /// 从内存中加载DLL
        /// </summary>
        /// <param name="buf"></param>
        /// <param name="nLen"></param>
        /// <returns></returns>
        [DllImport("kernel.dll", EntryPoint = "MemLoadLibrary", SetLastError = true)]
        public extern static IntPtr MemLoadLibrary(ref byte[] buf, int nLen);

        /// <summary>
        /// 获取内存中的DLL中函数的地址
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="funcName"></param>
        /// <returns></returns>
        [DllImport("kernel.dll", EntryPoint = "MemGetProcAddress", SetLastError = true)]
        public extern static IntPtr MemGetProcAddress(IntPtr handle, String funcName);

        /// <summary>
        /// 释放从内存中加载的DLL
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        [DllImport("kernel.dll", EntryPoint = "MemFreeLibrary", SetLastError = true)]
        public extern static bool MemFreeLibrary(IntPtr handle);

        /// <summary>
        /// 禁用鼠标键盘
        /// </summary>
        /// <param name="Block"></param>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool BlockInput(bool Block);

        [DllImport("user32", EntryPoint = "ScreenToClient", SetLastError = true)]
        public static extern int ScreenToClient(int hwnd, ref POINT lpPoint);

        [DllImport("user32", EntryPoint = "ScreenToClient", SetLastError = true)]
        public static extern int ScreenToClient(IntPtr hwnd, ref POINT lpPoint);

        [DllImport("user32", EntryPoint = "ClientToScreen", SetLastError = true)]
        public static extern int ClientToScreen(int hwnd, ref POINT lpPoint);

        [DllImport("user32", EntryPoint = "ClientToScreen", SetLastError = true)]
        public static extern int ClientToScreen(IntPtr hwnd, ref POINT lpPoint);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(int hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetExitCodeProcess(int dwProcessId, ref int exitCode);

        /// <summary>
        /// 获取鼠标双击事件间隔
        /// </summary>
        /// <returns>以毫秒表示的鼠标双击事件事件间隔</returns>
        [DllImport("user32.dll", EntryPoint = "GetDoubleClickTime", SetLastError = true)]
        public extern static int GetDoubleClickTime();

        [DllImport("user32.dll", EntryPoint = "GetDlgCtrlID", CharSet = CharSet.Auto, SetLastError = true)]
        public extern static int GetDlgCtrlID(int hWnd);


        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int FileTimeToSystemTime(IntPtr lpFileTime, IntPtr lpSystemTime);

        [DllImport("wininet.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr FindFirstUrlCacheEntry([MarshalAs(UnmanagedType.LPTStr)] string lpszUrlSearchPattern, IntPtr lpFirstCacheEntryInfo, ref int lpdwFirstCacheEntryInfoBufferSize);

        [DllImport("wininet.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool FindNextUrlCacheEntry(IntPtr hEnumHandle, IntPtr lpNextCacheEntryInfo, ref int lpdwNextCacheEntryInfoBufferSize);

        [DllImport("wininet.dll")]
        public static extern bool FindCloseUrlCache(IntPtr hEnumHandle);
    }
}
