namespace Mock.Tools.Controls
{
    using System;
    using System.Windows.Automation;
    using System.Collections.Generic;

    using Mock.Tools.Exception;
    using Mock.Nature.Native;
    internal class MessageBoxObject : WinObject
    {
        private string _messageBoxName = null;
        private int _processId = 0;
        internal MessageBoxObject(string messageBoxName)
        {
            #region old realization method
            //Condition condition;
            //if (string.IsNullOrEmpty(messageBoxName))
            //{
            //    condition = new AndCondition(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window), new PropertyCondition(AutomationElement.ClassNameProperty, "#32770"));
            //}
            //else
            //{
            //    condition = new AndCondition(new PropertyCondition(AutomationElement.NameProperty, messageBoxName), new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window), new PropertyCondition(AutomationElement.ClassNameProperty, "#32770"));
            //}

            //if (string.IsNullOrEmpty(parentName) || parentName == "桌面")
            //{
            //    element = AutomationElement.RootElement.FindFirst(TreeScope.Descendants, condition);
            //}
            //else
            //{
            //    GetElement(parentName);
            //    element = element.FindFirst(TreeScope.Children, condition);
            //}

            //if (element == null)
            //{
            //    throw new NullControlException(messageBoxName);
            //}
            //_elementName = element.Current.Name;
            #endregion

            if (string.IsNullOrEmpty(messageBoxName))
            {
                _windowName = "WMessageBox";
            }
            else
            {
                _windowName = messageBoxName;
            }
            _messageBoxName = messageBoxName;
            element = null;
        }

        internal MessageBoxObject(WindowsUnit e)
        {
            element = e;
        }

        internal MessageBoxObject(int processId, string messageBoxName = null)
        {
            if (string.IsNullOrEmpty(messageBoxName))
            {
                _windowName = "WMessageBox";
            }
            else
            {
                _windowName = messageBoxName;
            }

            _messageBoxName = messageBoxName;
            _processId = processId;
            element = null;
        }

        internal void Search()
        {
            if (element != null && CheckType(element))
            {
                return;
            }
            try
            {
                Robot.ExecuteWithTimeOut(() =>
                {
                    while (!Find())
                    {
                        Robot.Recess(1000);
                    }
                }, 60000);

            }
            catch (TimeOutException)
            {
                throw new TimeOutException("Search WMessageBox");
            }
            if (!Find())
            {
                throw new NullControlException("WMessageBox");
            }
        }

        internal bool Find()
        {
            List<WindowsUnit> activeWindowList = GetActiveWindow();

            for (int i = 0; i < activeWindowList.Count; i++)
            {
                try
                {
                    WindowsUnit window = activeWindowList[i];

                    if (!CheckBaseInfo(window))
                    {
                        continue;
                    }

                    if (CheckType(window))
                    {
                        element = window;
                        return true;
                    }
                }
                catch (System.Threading.ThreadAbortException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    LogManager.ErrorOnlyPrint(ex);
                }
            }
            return false;
        }

        private bool CheckBaseInfo(WindowsUnit unit)
        {
            try
            {
                if (!string.Equals(unit.Current.ClassName, "#32770"))
                {
                    //LogManager.Debug("ClassName not #32770");
                    return false;
                }
                if (!string.IsNullOrEmpty(_messageBoxName) && !string.Equals(unit.Current.Name, _messageBoxName))
                {
                    //LogManager.Debug("Name not equal");
                    return false;
                }

                if (_processId != 0 && unit.Current.ProcessId != _processId)
                {
                    //LogManager.Debug("ProcessId not equal");
                    return false;
                }
                return true;
            }
            catch (System.Threading.ThreadAbortException)
            {
                throw;
            }
            catch (Exception ex)
            {
                LogManager.ErrorOnlyPrint(ex);
                return false;
            }
        }

        private bool CheckType(WindowsUnit unit)
        {
            try
            {
                int btnNum = 0;
                WindowsUnit child = TreeWalker.RawViewWalker.GetFirstChild(unit);
                int txtNum = 0;
                while (child != null)
                {
                    if (child.Current.ControlType == ControlType.Button)
                    {
                        btnNum++;
                    }
                    else if (child.Current.ControlType == ControlType.Image)
                    {
                    }
                    else if (child.Current.ControlType == ControlType.Text)
                    {
                        if (txtNum < 1)
                        {
                            txtNum++;
                        }
                        else
                        {
                            //LogManager.Debug("Text length > 1");
                            return false;
                        }
                    }
                    else if (child.Current.ControlType == ControlType.TitleBar)
                    {
                        WindowsUnit tmp = child.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.AutomationIdProperty, "Maximize"));
                        if (tmp != null) return false;
                    }
                    else
                    {
                        //LogManager.Debug("Invalid Type {0}", child.Current.ControlType.ProgrammaticName);
                        return false;
                    }
                    child = TreeWalker.RawViewWalker.GetNextSibling(child);
                }
                if (btnNum > 0 && txtNum > 0)
                {
                    return true;
                }
                else
                {
                    //LogManager.Debug("no Button or Text");
                    return false;
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                throw;
            }
            catch(Exception)
            {
                //LogManager.ErrorOnlyPrint(ex);
                return false;
            }
        }

        internal string Message
        {
            get
            {
                Search();
                string msg = string.Empty;
                Robot.Recess(1000);
                WindowsUnit e = element.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Text));
                if (e == null)
                {
                    throw new NullControlException(_elementName, "WMessageBox.Message");
                }
                
                msg = e.Current.Name;
                LogManager.Debug(msg);
                return msg;
            }
        }

        internal void Click(string buttonName)
        {
            Search();
            if (element == null)
            {
                throw new NullControlException("MessageBox");
            }
            if (!element.Current.IsEnabled)
            {
                throw new ControlUnableException(_windowName, buttonName);
            }
            AndCondition condition = new AndCondition(new PropertyCondition(AutomationElement.NameProperty, buttonName), new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button));
            WindowsUnit button = element.FindFirst(TreeScope.Children, condition);
            if (button == null)
            {
                AutomationElementCollection collection = element.FindAll(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button));
                
                foreach (AutomationElement ec in collection)
                {
                    WindowsUnit e = ec;
                    if (string.Equals(e.Current.Name.Trim(), buttonName))
                    {
                        button = e;
                        break;
                    }
                }

                if (button == null)
                {
                    LogManager.Debug("MessageBox.Button element is null");
                    throw new NullControlException(_windowName, buttonName);
                }
            }
            
            InvokePattern invokePattern = button.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;

            if (invokePattern == null)
            {
                throw new UnClickableException();
            }

            try
            {
                SetCursor(button);
                invokePattern.Invoke();
            }
            catch (System.Threading.ThreadAbortException)
            {
                throw;
            }
            catch
            {
                Robot.Recess(500);
                NativeMethods.PostMessage(button.Current.NativeWindowHandle, WindowsMessages.BM_CLICK, 0, 0);
            }
            //Wait(1000);
        }

        internal void Click(bool ok)
        {
            //if (!element.Current.IsEnabled)
            //{
            //    throw new ControlUnableException(_windowName, _elementName);
            //}
            Search();
            Condition condition = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button);
            AutomationElementCollection buttonCollection = element.FindAll(TreeScope.Children, condition);
            if (buttonCollection == null)
            {
                throw new NullControlException(_windowName, "Button");
            }

            WindowsUnit button = null;

            if (ok)
            {
                //肯定意义按钮
                foreach (AutomationElement tmp in buttonCollection)
                {
                    WindowsUnit tmpBtn = tmp;
                    string tmpName = tmpBtn.Current.Name;
                    if (tmpName.ToUpper().Contains("Y")
                        || tmpName.Contains("是")
                        || tmpName.Contains("确定")
                        || tmpName.Contains("确认"))
                    {
                        button = tmpBtn;
                        break;
                    }
                }

                if (button == null)
                {
                    throw new NullControlException(_windowName, "肯定意义");
                }
            }
            else
            {
                //否定意义按钮
                foreach (AutomationElement tmp in buttonCollection)
                {
                    WindowsUnit tmpBtn = tmp;
                    string tmpName = tmpBtn.Current.Name;
                    if (tmpName.ToUpper().Contains("N")
                        || tmpName.Contains("否")
                        || tmpName.Contains("取消"))
                    {
                        button = tmpBtn;
                        break;
                    }
                }

                if (button == null)
                {
                    throw new NullControlException(_windowName, "否定意义");
                }
            }

            InvokePattern invokePattern = button.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;

            if (invokePattern == null)
            {
                throw new UnClickableException();
            }

            //SetCursor(button);
            //invokePattern.Invoke();
            //Wait(1000);
            try
            {
                SetCursor(button);
                invokePattern.Invoke();
            }
            catch (System.Threading.ThreadAbortException)
            {
                throw;
            }
            catch
            {
                Robot.Recess(500);
                NativeMethods.PostMessage(button.Current.NativeWindowHandle, WindowsMessages.BM_CLICK, 0, 0);
            }
        }

        internal void Close()
        {
            Search();
            WindowPattern wp = element.GetCurrentPattern(WindowPattern.Pattern) as WindowPattern;
            if (wp == null)
            {
                throw new UnSupportPatternException();
            }

            while (wp.Current.WindowInteractionState != WindowInteractionState.ReadyForUserInteraction)
            {
                CloseChild(element);
                Wait(1000);
            }
            wp.Close();
            Wait(1000);
        }

        private void CloseChild(WindowsUnit e)
        {
            AutomationElementCollection eles = e.FindAll(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window));
            foreach (AutomationElement ele in eles)
            {
                WindowPattern wp = ele.GetCurrentPattern(WindowPattern.Pattern) as WindowPattern;
                if (wp == null)
                {
                    throw new UnSupportPatternException();
                }
                while (wp.Current.WindowInteractionState != WindowInteractionState.ReadyForUserInteraction)
                {
                    CloseChild(ele);
                    Wait(1000);
                }
                wp.Close();
            }
        }
    }
}
