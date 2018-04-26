namespace Mock.Tools.Controls
{
    using System;
    using System.Collections.Generic;
    using Mock.Nature.Native;
    using System.Windows.Automation;
    using Mock.Tools.Exception;
    internal class HxDialogObject : WinObject
    {
        internal HxDialogObject(string parentName = null)
        {
            _elementName = "HxDialog";
        }

        internal HxDialogObject(WindowsUnit e)
        {
            _elementName = "HxDialog";
            element = e;
        }

        internal void Search()
        {
            try
            {
                if (element != null && element.Current.AutomationId == "SysMessageBox") return;
            }
            catch (System.Threading.ThreadAbortException)
            {
                throw;
            }
            catch { }
            try
            {
                Robot.ExecuteWithTimeOut(() =>
                {
                    while (!Find())
                    {
                        Robot.Recess(200);
                    }
                }, 60000);
            }
            catch (System.Threading.ThreadAbortException)
            {
                throw;
            }
            catch (Exception ex)
            {
                LogManager.ErrorOnlyPrint(ex);
                throw new TimeOutException("Search HxDialog");
            }
            LogManager.Debug("Search HxDialog success");
            if (!Find())
            {
                throw new NullControlException("HxDialog");
            }
        }

        internal bool Find()
        {
            IntPtr warningHandle = NativeMethods.FindWindow(null, "SysMessageBox");
            if (warningHandle != IntPtr.Zero)
            {
                element = AutomationElement.FromHandle(warningHandle);
                return true;
            }

            List<WindowsUnit> activeWindowList = GetActiveWindow();
            for (int i = 0; i < activeWindowList.Count; i++)
            {
                try
                {
                    WindowsUnit window = activeWindowList[i];
                    if (string.Equals(window.Current.AutomationId, "SysMessageBox"))
                    {
                        element = window;
                        return true;
                    }
                }
                catch (System.Threading.ThreadAbortException)
                {
                    throw;
                }
                catch { }
            }
            return false;
        }

        internal string Message
        {
            get
            {
                Search();
                bool find = false;
                for (int i = 0; i < Config.RedoCount; i++)
                {
                    try
                    {
                        string msg = string.Empty;
                        WindowsUnit e = element.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.AutomationIdProperty, "pnlForm"));
                        if (e == null)
                        {
                            throw new NullControlException(_elementName, "文本");
                        }
                        e = e.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.AutomationIdProperty, "BodyBounds"));
                        e = e.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.AutomationIdProperty, "BodyClient"));
                        e = e.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.AutomationIdProperty, "pnlMid"));
                        e = e.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.AutomationIdProperty, "lblMsg"));

                        msg = e.Current.Name;
                        find = true;
                        return msg;
                    }
                    catch (System.Threading.ThreadAbortException)
                    {
                        throw;
                    }
                    catch
                    {
                        Robot.Recess(1000);
                    }
                }
                if (!find)
                {
                    throw new NullControlException(_elementName, "文本");
                }
                return null;
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
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    if (element.Current.IsEnabled)
                    {
                        wp.Close();
                    }
                    Robot.Recess(100);
                }
                catch (System.Threading.ThreadAbortException)
                {
                    throw;
                }
                catch
                {
                    break;
                }
            }
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

        internal void Click(string buttonName)
        {
            Search();
            if (!element.Current.IsKeyboardFocusable)
            {
                throw new ControlUnableException(_elementName, buttonName);
            }
            WindowsUnit e = element.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.AutomationIdProperty, "pnlForm"));
            if (e == null)
            {
                throw new NullControlException(_elementName, "文本");
            }
            e = e.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.AutomationIdProperty, "BodyBounds"));
            e = e.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.AutomationIdProperty, "BodyClient"));
            e = e.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.AutomationIdProperty, "pnlBottom"));
            
            AndCondition condition = new AndCondition(new PropertyCondition(AutomationElement.NameProperty, buttonName), new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button));
            WindowsUnit button = e.FindFirst(TreeScope.Children, condition);
            if (button == null)
            {
                throw new NullControlException(_elementName, buttonName);
            }
            InvokePattern invokePattern = button.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;

            if (invokePattern == null)
            {
                throw new UnClickableException();
            }

            SetCursor(button);
            try
            {
                Robot.ExecuteWithTimeOut(() =>
                    {
                        invokePattern.Invoke();
                    }, 1000);
            }
            catch (System.Threading.ThreadAbortException)
            {
                throw;
            }
            catch { }
            
            for (int i = 0; i < Config.RedoCount; i++)
            {
                try
                {
                    LogManager.DebugFormat("{0} {1}", element.Current.Name, i);
                    if (element.Current.IsEnabled)
                    {
                        Robot.ExecuteWithTimeOut(() =>
                        {
                            invokePattern.Invoke();
                        }, 2000);
                    }
                    Robot.Recess(500);
                }
                catch (TimeOutException)
                {
                    continue;
                }
                catch (System.Threading.ThreadAbortException)
                {
                    throw;
                }
                catch
                {
                    break;
                }
            }
        }

        internal void Click(bool des)
        {
            Search();
            if (!element.Current.IsKeyboardFocusable)
            {
                throw new ControlUnableException(_elementName, "肯定意义");
            }

            WindowsUnit e = element.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.AutomationIdProperty, "pnlForm"));

            for (int i = 0; i < Config.RedoCount && e == null; i++)
            {
                Robot.Recess(100);
                e = element.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.AutomationIdProperty, "pnlForm"));
            }
            if (e == null)
            {
                throw new NullControlException(_elementName, "文本");
            }
            e = e.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.AutomationIdProperty, "BodyBounds"));
            e = e.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.AutomationIdProperty, "BodyClient"));
            e = e.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.AutomationIdProperty, "pnlBottom"));

            PropertyCondition condition = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button);
            AutomationElementCollection buttonList = e.FindAll(TreeScope.Children, condition);
            if (buttonList == null)
            {
                throw new NullControlException(_elementName, "肯定意义");
            }

            WindowsUnit button = null;
            foreach (AutomationElement b1 in buttonList)
            {
                WindowsUnit b = b1;
                string aid = b.Current.AutomationId.Trim().ToLower();
                if (des)
                {
                    if (aid.Contains("yes")
                    || aid.Contains("ok"))
                    {
                        button = b;
                        break;
                    }
                }
                else
                {
                    if (aid.Contains("no")
                 || aid.Contains("cancel"))
                    {
                        button = b;
                        break;
                    }
                }
            }
            if (button == null)
            {
                throw new NullControlException(_elementName, "肯定意义");
            }
            InvokePattern invokePattern = button.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;

            if (invokePattern == null)
            {
                throw new UnClickableException();
            }

            SetCursor(button);
            //invokePattern.Invoke();
            try
            {
                Robot.ExecuteWithTimeOut(() =>
                {
                    invokePattern.Invoke();
                }, 1000);
            }
            catch (System.Threading.ThreadAbortException)
            {
                throw;
            }
            catch { }

            for (int i = 0; i < Config.RedoCount; i++)
            {
                try
                {
                    LogManager.DebugFormat("{0} {1}", element.Current.Name, i);
                    if (element.Current.IsEnabled)
                    {
                        Robot.ExecuteWithTimeOut(() =>
                        {
                            invokePattern.Invoke();
                        }, 2000);
                    }
                    Robot.Recess(500);
                }
                catch (TimeOutException)
                {
                    continue;
                }
                catch (System.Threading.ThreadAbortException)
                {
                    throw;
                }
                catch
                {
                    break;
                }
            }
        }
    }
}
