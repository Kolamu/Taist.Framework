namespace Mock.Data
{
    /// <summary>
    /// 键盘模拟按键
    /// </summary>
    public enum VK
    {
        /// <summary>
        /// 退格键
        /// </summary>
        BACK = 8,

        /// <summary>
        /// Tab键
        /// </summary>
        TAB = 9,

        /// <summary>
        /// Shift键
        /// </summary>
        SHIFT = 10,

        /// <summary>
        /// 回车键
        /// </summary>
        RETURN = 13,

        /// <summary>
        /// Ctrl键
        /// </summary>
        CTRL = 17,

        /// <summary>
        /// Alt键
        /// </summary>
        ALT = 18,

        /// <summary>
        /// Pause Break鍵
        /// </summary>
        PAUSE = 19,

        /// <summary>
        /// Caps_Lock键
        /// </summary>
        CAPS_LOCK = 20,

        /// <summary>
        /// Esc键
        /// </summary>
        ESC = 27,

        /// <summary>
        /// 空格键
        /// </summary>
        SPACE = 32,

        /// <summary>
        /// Page Up键
        /// </summary>
        PAGE_UP = 33,

        /// <summary>
        /// Page Down键
        /// </summary>
        PAGE_DOWN = 34,

        /// <summary>
        /// End键
        /// </summary>
        END = 35,

        /// <summary>
        /// Home键
        /// </summary>
        HOME = 36,

        /// <summary>
        /// 左键
        /// </summary>
        LEFT = 37,

        /// <summary>
        /// 上键
        /// </summary>
        UP = 38,

        /// <summary>
        /// 右键
        /// </summary>
        RIGHT = 39,

        /// <summary>
        /// 下键
        /// </summary>
        DOWN = 40,

        /// <summary>
        /// Insert键
        /// </summary>
        INSERT = 45,

        /// <summary>
        /// Delete键
        /// </summary>
        DELETE = 46,

        /// <summary>
        /// A键
        /// </summary>
        A = 65,
        B,
        C,
        D,
        E,
        F,
        G,
        H,
        I,
        J,
        K,
        L,
        M,
        N,
        O,
        P,
        Q,
        R,
        S,
        T,
        U,
        V,
        W,
        X,
        Y,
        Z,

        /// <summary>
        /// 左徽标键
        /// </summary>
        LWIN = 91,

        /// <summary>
        /// 右徽标键
        /// </summary>
        RWIN = 92,

        /// <summary>
        /// 右键菜单键
        /// </summary>
        APPS = 93,

        /// <summary>
        /// 小键盘0键
        /// </summary>
        NUMPAD0 = 96,

        /// <summary>
        /// 小键盘1键
        /// </summary>
        NUMPAD1 = 97,

        /// <summary>
        /// 小键盘2键
        /// </summary>
        NUMPAD2 = 98,

        /// <summary>
        /// 小键盘3键
        /// </summary>
        NUMPAD3 = 99,

        /// <summary>
        /// 小键盘4键
        /// </summary>
        NUMPAD4 = 100,

        /// <summary>
        /// 小键盘5键
        /// </summary>
        NUMPAD5 = 101,

        /// <summary>
        /// 小键盘6键
        /// </summary>
        NUMPAD6 = 102,

        /// <summary>
        /// 小键盘7键
        /// </summary>
        NUMPAD7 = 103,

        /// <summary>
        /// 小键盘8键
        /// </summary>
        NUMPAD8 = 104,

        /// <summary>
        /// 小键盘9键
        /// </summary>
        NUMPAD9 = 105,

        /// <summary>
        /// 小键盘*键
        /// </summary>
        MULTIPLY = 106,

        /// <summary>
        /// 小键盘+键
        /// </summary>
        ADD = 107,

        /// <summary>
        /// 小键盘-键
        /// </summary>
        SUBTRACT = 109,

        /// <summary>
        /// 小键盘.键
        /// </summary>
        DECIMAL = 110,

        /// <summary>
        /// 小键盘/键
        /// </summary>
        DIVIDE = 111,

        /// <summary>
        /// F1键
        /// </summary>
        F1 = 112,

        /// <summary>
        /// F2键
        /// </summary>
        F2 = 113,

        /// <summary>
        /// F3鍵
        /// </summary>
        F3 = 114,

        /// <summary>
        /// F4鍵
        /// </summary>
        F4 = 115,

        /// <summary>
        /// F5鍵
        /// </summary>
        F5 = 116,

        /// <summary>
        /// F6鍵
        /// </summary>
        F6 = 117,

        /// <summary>
        /// F7鍵
        /// </summary>
        F7 = 118,

        /// <summary>
        /// F8鍵
        /// </summary>
        F8 = 119,

        /// <summary>
        /// F9鍵
        /// </summary>
        F9 = 120,

        /// <summary>
        /// F10鍵
        /// </summary>
        F10 = 121,

        /// <summary>
        /// F11鍵
        /// </summary>
        F11 = 122,

        /// <summary>
        /// F12鍵
        /// </summary>
        F12 = 123,

        /// <summary>
        /// Num Lock鍵
        /// </summary>
        NUM_LOCK = 144,

        /// <summary>
        /// Scroll Lock鍵
        /// </summary>
        SCROLL = 145,

    }

    /// <summary>
    /// 数据转换类型
    /// </summary>
    public enum ConvertType
    {
        CLASS,
        VALUE,
        ENCRYPT,
        DECRYPT
    }

   /// <summary>
   /// 加密类型
   /// </summary>
    public enum EncryptType
    {
        MD5,
        AES,
        RSA,
        BASE64
    }

    /// <summary>
    /// 解密类型
    /// </summary>
    public enum DecryptType
    {
        AES,
        RSA,
        BASE64
    }
}
