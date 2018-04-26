namespace Mock.Tools.Controls
{
    /// <summary>
    /// 表示面板操作对象
    /// </summary>
    public class WPanel
    {
        /// <summary>
        /// 点击面板
        /// </summary>
        /// <param name="windowName"></param>
        /// <param name="panelName"></param>
        public static void Click(string windowName, string panelName)
        {
            PanelObject panelObj = new PanelObject(windowName, panelName);
            panelObj.Click();
            panelObj = null;
        }

        /// <summary>
        /// 点击面板
        /// </summary>
        /// <param name="windowName"></param>
        /// <param name="panelName"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void Click(string windowName, string panelName, int x, int y)
        {
            PanelObject panelObj = new PanelObject(windowName, panelName);
            panelObj.Click(x, y);
            panelObj = null;
        }

        /// <summary>
        /// 设置面板焦点
        /// </summary>
        /// <param name="windowName"></param>
        /// <param name="panelName"></param>
        public static void SetFocus(string windowName, string panelName)
        {
            PanelObject panelObje = new PanelObject(windowName, panelName);
            panelObje.SetFocus();
            panelObje = null;
        }

        /// <summary>
        /// 获取面板文本
        /// </summary>
        /// <param name="windowName"></param>
        /// <param name="panelName"></param>
        /// <returns></returns>
        public static string GetText(string windowName, string panelName)
        {
            PanelObject panelObje = new PanelObject(windowName, panelName);
            string ret = panelObje.GetText();
            panelObje = null;
            return ret;
        }

        /// <summary>
        /// 面板中输入
        /// </summary>
        /// <param name="windowName"></param>
        /// <param name="panelName"></param>
        /// <param name="msg"></param>
        public static void Input(string windowName, string panelName, string msg)
        {
            if (string.Equals(msg, "NOTINPUT", System.StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            PanelObject panelObje = new PanelObject(windowName, panelName);
            panelObje.Input(msg);
            panelObje = null;
        }

        /// <summary>
        /// 查看Panel是否存在
        /// </summary>
        /// <param name="windowName"></param>
        /// <param name="panelName"></param>
        /// <returns></returns>
        public static bool Exist(string windowName, string panelName)
        {
            try
            {
                new PanelObject(windowName, panelName);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
