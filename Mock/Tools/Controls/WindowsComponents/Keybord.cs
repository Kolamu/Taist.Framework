namespace Mock.Tools.Controls
{
    using Mock.Data;
    /// <summary>
    /// 表示键盘对象
    /// </summary>
    public class Keybord
    {
        /// <summary>
        /// 模拟键盘输入字符串
        /// </summary>
        /// <param name="s">输入的字符串</param>
        public static void Input(string s)
        {
            if (string.Equals(s, "NOTINPUT", System.StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            KeybordObject ko = new KeybordObject();
            ko.Input(s);
            ko = null;
        }

        /// <summary>
        /// 按下指定键
        /// </summary>
        /// <param name="vk">虚拟键值</param>
        public static void KeyDown(VK vk)
        {
            KeybordObject ko = new KeybordObject();
            ko.KeyDown(vk);
            ko = null;
        }

        /// <summary>
        /// 弹起指定键
        /// </summary>
        /// <param name="vk">虚拟键值</param>
        public static void KeyUp(VK vk)
        {
            KeybordObject ko = new KeybordObject();
            ko.KeyUp(vk);
            ko = null;
        }

        /// <summary>
        /// 敲击指定键
        /// </summary>
        /// <param name="vk"></param>
        public static void Press(VK vk)
        {
            KeybordObject ko = new KeybordObject();
            ko.KeyDown(vk);
            ko.KeyUp(vk);
            ko = null;
        }
    }
}
