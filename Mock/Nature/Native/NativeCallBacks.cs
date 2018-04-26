namespace Mock.Nature.Native
{
    using System;

    /// <summary>
    /// Hook 操作句柄
    /// </summary>
    /// <param name="code">操作码</param>
    /// <param name="wparam">wParam</param>
    /// <param name="lparam">lParam</param>
    /// <returns></returns>
    public delegate IntPtr HookProc(int code, IntPtr wparam, IntPtr lparam);

    /// <summary>
    /// 枚举窗口句柄
    /// </summary>
    /// <param name="hWnd"></param>
    /// <param name="lParam"></param>
    /// <returns></returns>
    public delegate bool EnumWindowsProc(IntPtr hWnd, int lParam);

    /// <summary>
    /// 枚举子窗口句柄
    /// </summary>
    /// <param name="hwnd">窗口句柄</param>
    /// <param name="lParam"></param>
    /// <returns></returns>
    public delegate bool EnumChildWindowsProc(IntPtr hwnd, int lParam);
}
