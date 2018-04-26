namespace Mock.Tools.Controls
{
    using System.Windows.Automation;
    using Mock.Tools.Exception;
    using System.Collections.Generic;
    internal class OpenSaveFileDialogObject : WinObject
    {
        internal OpenSaveFileDialogObject(string windowName)
        {
            bool find = false;
            RobotContext.WindowName = windowName;
            for (int i = 0; i < 10 && !find; i++)
            {
                List<WindowsUnit> activeWindowList = GetActiveWindow();
                foreach (WindowsUnit window in activeWindowList)
                {
                    if (window.Current.Name == windowName)
                    {
                        element = window;
                        RobotContext.Window = window;
                        find = true;
                        break;
                    }
                }
                Wait(1000);
            }
            if (!find)
            {
                throw new NullControlException(windowName);
            }
        }

        internal void SetFilePath(string path)
        {
            WindowsUnit unit = element.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ClassNameProperty, "DUIViewWndClassName"));
            if (unit == null)
            {
                throw new NullControlException("DUIViewWndClassName");
            }

            unit = unit.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.AutomationIdProperty, "FileNameControlHost"));
            if (unit == null)
            {
                unit = element.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ComboBox));
                if (unit == null)
                {
                    throw new NullControlException("FileNameControlHost");
                }

                if (!unit.Current.Name.Contains("文件名"))
                {
                    throw new NullControlException("文件名");
                }
            }

            unit = unit.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit));
            if (unit == null)
            {
                throw new NullControlException("文件名");
            }

            EditObject edit = new EditObject(unit);
            edit.Input(path);
        }

        internal void Click(string buttonName)
        {
            WindowsUnit button = element.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, buttonName));
            if (button == null)
            {
                throw new NullControlException(buttonName);
            }

            if (button.Current.ControlType == ControlType.Button)
            {
                ButtonObject btn = new ButtonObject(button);
                btn.Click();
            }
            else
            {
                SetCursor(button, true);
            }
        }
    }
}
