namespace Mock.Tools.Controls
{
    using System.Windows.Automation;
    using Mock.Tools.Exception;
    using Mock.Nature.Native;
    /// <summary>
    /// <c>ButtonObject</c>
    /// 表示一个按钮对象
    /// 
    /// </summary>
    internal sealed class ButtonObject : WinObject
    {
        #region 构造函数
        /// <summary>
        ///根据窗口名称和按钮名称获得按钮
        /// </summary>
        /// <param name="windowName">按钮所在窗口的虚拟名称，这通常在对象库中显示为Window的FriendlyName属性</param>
        /// <param name="buttonName">按钮名称，这通常在对象库中显示为Element的FriendlyName属性</param>
        internal ButtonObject(string windowName, string buttonName)
        {
            _windowName = windowName;
            _elementName = buttonName;
            WWindow.SearchWindow(windowName);
            GetElement(windowName, buttonName);
            if (!element.Current.ControlType.Equals(ControlType.Button))
            {
                element = element.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button));
                if (element == null)
                {
                    throw new NullControlException(windowName, buttonName);
                }
            }
        }

        public ButtonObject(WindowsUnit e)
        {
            element = e;
        }

        #endregion


        internal void Click()
        {
            //if (!element.Current.IsKeyboardFocusable)
            //{
            //    throw new ControlUnableException(_windowName, _elementName);
            //}
            InvokePattern invokePattern = element.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;

            if (invokePattern == null)
            {
                throw new UnClickableException();
            }

            TaistControlEventInfo info = new TaistControlEventInfo();
            info.ControlType = TaistControlType.Button;
            info.EventType = TaistEventType.Click;
            info.EventTime = System.DateTime.Now;

            RobotContext.LastAction(() =>
                {
                    LogManager.Debug("=============> Do click button");
                    element.SetFocus();
                    invokePattern.Invoke();
                    return null;
                });
            //Nature.Native.NativeMethods.PostMessage((System.IntPtr)element.Current.NativeWindowHandle, WindowsMessages.BM_CLICK, 0, 0);
            
            RobotContext.RunControlEvent(info);
        }

        internal void ClickByMouse()
        {
            if (!element.Current.IsKeyboardFocusable)
            {
                throw new ControlUnableException(_windowName, _elementName);
            }
            //InvokePattern invokePattern = element.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;

            //if (invokePattern == null)
            //{
            //    throw new UnClickableException();
            //}

            TaistControlEventInfo info = new TaistControlEventInfo();
            info.ControlType = TaistControlType.Button;
            info.EventType = TaistEventType.Click;
            info.EventTime = System.DateTime.Now;
            //invokePattern.Invoke();
            //Nature.Native.NativeMethods.PostMessage((System.IntPtr)element.Current.NativeWindowHandle, WindowsMessages.BM_CLICK, 0, 0);
            WWindow.SetTopMost(RobotContext.WindowName);
            RobotContext.LastAction(() =>
                {
                    LogManager.Debug("=============> Do click button");
                    element.SetFocus();
                    System.Windows.Rect rect = element.Current.BoundingRectangle;
                    Mouse.Click((int)(rect.X + rect.Width / 2), (int)(rect.Y + rect.Height / 2));
                    Keybord.Press(Data.VK.RETURN);
                    return null;
                });
            //Wait(100);
            RobotContext.RunControlEvent(info);
        }

        internal void DoClick()
        {

        }
    }
}
