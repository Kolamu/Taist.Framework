namespace Mock.Nature.Native
{
    /// <summary>
    /// Hook类型
    /// </summary>
    public enum HookType
    {
        /// <summary>
        /// 对话框、消息框、菜单或滚动条输入消息钩子
        /// </summary>
        MsgFilter = -1,

        /// <summary>
        /// 输入消息记录钩子
        /// </summary>
        JournalRecord    = 0,

        /// <summary>
        /// 回放通过JournalRecord钩子记录的输入消息
        /// </summary>
        JournalPlayback  = 1,

        /// <summary>
        /// 键盘消息钩子
        /// </summary>
        Keyboard         = 2,

        /// <summary>
        /// 接收消息投递钩子
        /// </summary>
        GetMessage       = 3,

        /// <summary>
        /// 系统将消息发送到指定窗口之前钩子
        /// </summary>
        CallWndProc      = 4,

        /// <summary>
        /// 基于计算机培训的钩子
        /// </summary>
        CBT              = 5,

        /// <summary>
        /// 系统消息钩子
        /// </summary>
        SysMsgFilter     = 6,

        /// <summary>
        /// 鼠标消息钩子
        /// </summary>
        Mouse            = 7,

        /// <summary>
        /// 非标准硬件(非鼠标、键盘)消息钩子
        /// </summary>
        Hardware         = 8,

        /// <summary>
        /// 差错钩子
        /// </summary>
        Debug            = 9,

        /// <summary>
        /// 外壳钩子
        /// </summary>
        Shell            = 10,

        /// <summary>
        /// 前台空闲窗口钩子
        /// </summary>
        ForegroundIdle   = 11,

        /// <summary>
        /// 消息已经在窗口中处理的钩子
        /// </summary>
        CallWndProcRet   = 12,

        /// <summary>
        /// 底层键盘消息钩子
        /// </summary>
        KeyboardLL       = 13,

        /// <summary>
        /// 底层鼠标消息钩子
        /// </summary>
        MouseLL          = 14
    }

    #region Accessibly
    /// <summary>
    /// MSAA OBJID
    /// </summary>
    public enum OBJID : uint
    {
        /// <summary>
        /// 窗口
        /// </summary>
        WINDOW = 0x00000000,

        /// <summary>
        /// 系统菜单
        /// </summary>
        SYSMENU = 0xFFFFFFFF,

        /// <summary>
        /// 标题栏
        /// </summary>
        TITLEBAR = 0xFFFFFFFE,

        /// <summary>
        /// 菜单
        /// </summary>
        MENU = 0xFFFFFFFD,

        /// <summary>
        /// 控件
        /// </summary>
        CLIENT = 0xFFFFFFFC,

        /// <summary>
        /// 垂直滚动条
        /// </summary>
        VSCROLL = 0xFFFFFFFB,

        /// <summary>
        /// 水平滚动条
        /// </summary>
        HSCROLL = 0xFFFFFFFA,

        /// <summary>
        /// 
        /// </summary>
        SIZEGRIP = 0xFFFFFFF9,

        /// <summary>
        /// 
        /// </summary>
        CARET = 0xFFFFFFF8,

        /// <summary>
        /// 鼠标指针
        /// </summary>
        CURSOR = 0xFFFFFFF7,

        /// <summary>
        /// 
        /// </summary>
        ALERT = 0xFFFFFFF6,

        /// <summary>
        /// 声音
        /// </summary>
        SOUND = 0xFFFFFFF5,
    }

    /// <summary>
    /// IAccessible.accSelect selectFlags
    /// </summary>
    public enum SELFLAG : uint
    {
        /// <summary>
        /// Performs no action. Active Accessibility does not change the selection or focus.
        /// </summary>
        NONE =0,

        /// <summary>
        /// Sets the focus to the object and makes it the selection anchor. 
        /// </summary>
        TAKEFOCUS = 1,

        /// <summary>
        /// Selects the object and removes the selection from all other objects in the container.
        /// </summary>
        TAKESELECTION = 2,

        /// <summary>
        /// Alters the selection so that all objects between the selection anchor and this object take on the anchor object's selection state.
        /// </summary>
        EXTENDSELECTION = 4,

        /// <summary>
        /// Adds the object to the current selection; possible result is a noncontiguous selection.
        /// </summary>
        ADDSELECTION = 8,

        /// <summary>
        /// Removes the object from the current selection; possible result is a noncontiguous selection. 
        /// </summary>
        REMOVESELECTION = 16
    }
    #endregion

    #region SetWindowLong indexes
    /// <summary>
    /// SetWindowLong indexes
    /// </summary>
    public enum SetWindowLongOffsets
    {
        /// <summary>
        /// GWL_WNDPROC
        /// </summary>
        GWL_WNDPROC = (-4),

        /// <summary>
        /// GWL_HINSTANCE
        /// </summary>
        GWL_HINSTANCE = (-6),

        /// <summary>
        /// GWL_HWNDPARENT
        /// </summary>
        GWL_HWNDPARENT = (-8),

        /// <summary>
        /// GWL_STYLE
        /// </summary>
        GWL_STYLE = (-16),

        /// <summary>
        /// GWL_STYLE
        /// </summary>
        GWL_EXSTYLE = (-20),

        /// <summary>
        /// GWL_STYLE
        /// </summary>
        GWL_USERDATA = (-21),

        /// <summary>
        /// GWL_STYLE
        /// </summary>
        GWL_ID = (-12)
    }
    #endregion

    #region Window Styles

    /// <summary>
    /// 窗口样式
    /// </summary>
    public enum WindowStyles : uint
    {
        /// <summary>
        /// 
        /// </summary>
        WS_OVERLAPPED = 0x00000000,

        /// <summary>
        /// 
        /// </summary>
        WS_POPUP = 0x80000000,

        /// <summary>
        /// 
        /// </summary>
        WS_CHILD = 0x40000000,

        /// <summary>
        /// 
        /// </summary>
        WS_MINIMIZE = 0x20000000,

        /// <summary>
        /// 
        /// </summary>
        WS_VISIBLE = 0x10000000,

        /// <summary>
        /// 
        /// </summary>
        WS_DISABLED = 0x08000000,

        /// <summary>
        /// 
        /// </summary>
        WS_CLIPSIBLINGS = 0x04000000,

        /// <summary>
        /// 
        /// </summary>
        WS_CLIPCHILDREN = 0x02000000,

        /// <summary>
        /// 
        /// </summary>
        WS_MAXIMIZE = 0x01000000,

        /// <summary>
        /// 
        /// </summary>
        WS_CAPTION = 0x00C00000,

        /// <summary>
        /// 
        /// </summary>
        WS_BORDER = 0x00800000,

        /// <summary>
        /// 
        /// </summary>
        WS_DLGFRAME = 0x00400000,

        /// <summary>
        /// 
        /// </summary>
        WS_VSCROLL = 0x00200000,

        /// <summary>
        /// 
        /// </summary>
        WS_HSCROLL = 0x00100000,

        /// <summary>
        /// 
        /// </summary>
        WS_SYSMENU = 0x00080000,

        /// <summary>
        /// 
        /// </summary>
        WS_THICKFRAME = 0x00040000,

        /// <summary>
        /// 
        /// </summary>
        WS_GROUP = 0x00020000,

        /// <summary>
        /// 
        /// </summary>
        WS_TABSTOP = 0x00010000,

        /// <summary>
        /// 
        /// </summary>
        WS_MINIMIZEBOX = 0x00020000,

        /// <summary>
        /// 
        /// </summary>
        WS_MAXIMIZEBOX = 0x00010000,

        /// <summary>
        /// 
        /// </summary>
        WS_TILED = 0x00000000,

        /// <summary>
        /// 
        /// </summary>
        WS_ICONIC = 0x20000000,

        /// <summary>
        /// 
        /// </summary>
        WS_SIZEBOX = 0x00040000,

        /// <summary>
        /// 
        /// </summary>
        WS_POPUPWINDOW = 0x80880000,

        /// <summary>
        /// 
        /// </summary>
        WS_OVERLAPPEDWINDOW = 0x00CF0000,

        /// <summary>
        /// 
        /// </summary>
        WS_TILEDWINDOW = 0x00CF0000,

        /// <summary>
        /// 
        /// </summary>
        WS_CHILDWINDOW = 0x40000000
    }

    /// <summary>
    /// 窗口样式
    /// </summary>
    public enum WindowExtStyles : uint
    {
        /// <summary>
        /// 
        /// </summary>
        WS_OVERLAPPED = 0x00000000,

        /// <summary>
        /// 
        /// </summary>
        WS_POPUP = 0x80000000,

        /// <summary>
        /// 
        /// </summary>
        WS_CHILD = 0x40000000,

        /// <summary>
        /// 
        /// </summary>
        WS_MINIMIZE = 0x20000000,

        /// <summary>
        /// 
        /// </summary>
        WS_VISIBLE = 0x10000000,

        /// <summary>
        /// 
        /// </summary>
        WS_DISABLED = 0x08000000,

        /// <summary>
        /// 
        /// </summary>
        WS_CLIPSIBLINGS = 0x04000000,

        /// <summary>
        /// 
        /// </summary>
        WS_CLIPCHILDREN = 0x02000000,

        /// <summary>
        /// 
        /// </summary>
        WS_MAXIMIZE = 0x01000000,

        /// <summary>
        /// 
        /// </summary>
        WS_CAPTION = 0x00C00000,

        /// <summary>
        /// 
        /// </summary>
        WS_BORDER = 0x00800000,

        /// <summary>
        /// 
        /// </summary>
        WS_DLGFRAME = 0x00400000,

        /// <summary>
        /// 
        /// </summary>
        WS_VSCROLL = 0x00200000,

        /// <summary>
        /// 
        /// </summary>
        WS_HSCROLL = 0x00100000,

        /// <summary>
        /// 
        /// </summary>
        WS_SYSMENU = 0x00080000,

        /// <summary>
        /// 
        /// </summary>
        WS_THICKFRAME = 0x00040000,

        /// <summary>
        /// 
        /// </summary>
        WS_GROUP = 0x00020000,

        /// <summary>
        /// 
        /// </summary>
        WS_TABSTOP = 0x00010000,

        /// <summary>
        /// 
        /// </summary>
        WS_MINIMIZEBOX = 0x00020000,

        /// <summary>
        /// 
        /// </summary>
        WS_MAXIMIZEBOX = 0x00010000,

        /// <summary>
        /// 
        /// </summary>
        WS_TILED = 0x00000000,

        /// <summary>
        /// 
        /// </summary>
        WS_ICONIC = 0x20000000,

        /// <summary>
        /// 
        /// </summary>
        WS_SIZEBOX = 0x00040000,

        /// <summary>
        /// 
        /// </summary>
        WS_POPUPWINDOW = 0x80880000,

        /// <summary>
        /// 
        /// </summary>
        WS_OVERLAPPEDWINDOW = 0x00CF0000,

        /// <summary>
        /// 
        /// </summary>
        WS_TILEDWINDOW = 0x00CF0000,

        /// <summary>
        /// 
        /// </summary>
        WS_CHILDWINDOW = 0x40000000
    }
    
    #endregion

    #region GetWindow indexes
    public enum GetWindowOffsets
    {
        /// <summary>
        /// The retrieved handle identifies the child window at the top of the Z order, if the specified window is a parent window; otherwise, the retrieved handle is NULL. The function examines only child windows of the specified window. It does not examine descendant windows.
        /// </summary>
        GW_CHILD = 5,
        /// <summary>
        /// The retrieved handle identifies the enabled popup window owned by the specified window (the search uses the first such window found using GW_HWNDNEXT); otherwise, if there are no enabled popup windows, the retrieved handle is that of the specified window.
        /// </summary>
        GW_ENABLEDPOPUP = 6,
        /// <summary>
        /// The retrieved handle identifies the window of the same type that is highest in the Z order.
        /// If the specified window is a topmost window, the handle identifies a topmost window. If the specified window is a top-level window, the handle identifies a top-level window. If the specified window is a child window, the handle identifies a sibling window.
        /// </summary>
        GW_HWNDFIRST = 0,

        /// <summary>
        /// The retrieved handle identifies the window of the same type that is lowest in the Z order.
        /// If the specified window is a topmost window, the handle identifies a topmost window. If the specified window is a top-level window, the handle identifies a top-level window. If the specified window is a child window, the handle identifies a sibling window.
        /// </summary>
        GW_HWNDLAST = 1,

        /// <summary>
        /// The retrieved handle identifies the window below the specified window in the Z order.
        /// If the specified window is a topmost window, the handle identifies a topmost window. If the specified window is a top-level window, the handle identifies a top-level window. If the specified window is a child window, the handle identifies a sibling window.
        /// </summary>
        GW_HWNDNEXT = 2,

        /// <summary>
        /// The retrieved handle identifies the window above the specified window in the Z order.
        /// If the specified window is a topmost window, the handle identifies a topmost window. If the specified window is a top-level window, the handle identifies a top-level window. If the specified window is a child window, the handle identifies a sibling window.
        /// </summary>
        GW_HWNDPREV = 3,

        /// <summary>
        /// The retrieved handle identifies the specified window's owner window, if any. For more information, see Owned Windows. 
        /// </summary>
        GW_OWNER = 4
    }
    #endregion

}
