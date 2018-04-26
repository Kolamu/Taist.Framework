namespace Mock.Tools.Controls
{
    /// <summary>
    /// 表示打开/保存对话框操作对象
    /// </summary>
    public class WOpenSaveFileDialog
    {
        /// <summary>
        /// 输入打开/保存对话框路径操作
        /// </summary>
        /// <param name="parentWindowName">父窗体名称</param>
        /// <param name="windowName">窗口名称</param>
        /// <param name="path">路径</param>
        public static void SetFilePath(string parentWindowName, string windowName, string path)
        {
            if (string.Equals(path, "NOTINPUT", System.StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            OpenSaveFileDialogObject osFileDiaObj = new OpenSaveFileDialogObject(windowName);
            osFileDiaObj.SetFilePath(path);
            osFileDiaObj = null;
        }

        /// <summary>
        /// 点击打开/保存对话框中按钮操作
        /// </summary>
        /// <param name="parentWindowName">父窗体名称</param>
        /// <param name="windowName">窗口名称</param>
        /// <param name="buttonName">按钮名称</param>
        public static void Click(string parentWindowName, string windowName, string buttonName)
        {
            OpenSaveFileDialogObject osFileDiaObj = new OpenSaveFileDialogObject(windowName);
            osFileDiaObj.Click(buttonName);
            osFileDiaObj = null;
        }
    }
}
