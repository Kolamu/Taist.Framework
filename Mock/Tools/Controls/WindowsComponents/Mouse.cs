namespace Mock.Tools.Controls
{
    /// <summary>
    /// 鼠标对象
    /// </summary>
    public class Mouse
    {
        /// <summary>
        /// 左键单击(x, y)点
        /// </summary>
        /// <param name="x">单击点的X坐标</param>
        /// <param name="y">单击点的Y坐标</param>
        public static void Click(int x, int y)
        {
            MouseObject mouse = new MouseObject(x, y);
            mouse.Click();
            mouse = null;
        }

        public static void Click(double x, double y)
        {
            MouseObject mouse = new MouseObject((int)x, (int)y);
            mouse.Click();
            mouse = null;
        }

        public static void Roll(bool up)
        {
            MouseObject mouse = new MouseObject(0, 0);
            mouse.Roll(up);
            mouse = null;
        }

        /// <summary>
        /// 左键双击(x, y)点
        /// </summary>
        /// <param name="x">双击点的X坐标</param>
        /// <param name="y">双击点的Y坐标</param>
        public static void DbClick(int x, int y)
        {
            MouseObject mouse = new MouseObject(x, y);
            mouse.DbClick();
            mouse = null;
        }

        /// <summary>
        /// 右键单击(x, y)点
        /// </summary>
        /// <param name="x">单击点的X坐标</param>
        /// <param name="y">单击点的Y坐标</param>
        public static void RightClick(int x, int y)
        {
            MouseObject mouse = new MouseObject(x, y);
            mouse.RightClick();
            mouse = null;
        }

        /// <summary>
        /// 移动鼠标指针到(x, y)点
        /// </summary>
        /// <param name="x">移动到点的X坐标</param>
        /// <param name="y">移动到点的Y坐标</param>
        public static void Move(int x, int y)
        {
            MouseObject mouse = new MouseObject(x, y);
            mouse.Move();
            mouse = null;
        }

        /// <summary>
        /// 获取鼠标的当前坐标
        /// </summary>
        public static Mock.Nature.Native.POINT Position
        {
            get
            {
                MouseObject mouse = new MouseObject();
                return mouse.Position;
            }
            set
            {
                MouseObject mouse = new MouseObject();
                mouse.Position = value;
            }
        }
    }
}
