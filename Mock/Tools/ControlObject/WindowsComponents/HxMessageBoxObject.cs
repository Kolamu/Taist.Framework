namespace Mock.Tools.Controls
{
    using Mock.Data.Exception;
    using System.Windows.Automation;
    using System.Collections.Generic;
    using Mock.Tools.Exception;
    internal class HxMessageBoxObject : WinObject
    {
        internal HxMessageBoxObject(string parentName = null)
        {
            _elementName = "航信提示框";
            element = null;
        }

        internal HxMessageBoxObject(WindowsUnit e)
        {
            _elementName = "航信提示框";
            element = e;
        }

        internal void Search()
        {
            if (element != null && element.Current.AutomationId == "CusMessageBox") return;

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
                throw new TimeOutException("Search HxMessageBox");
            }
            if (!Find())
            {
                throw new NullControlException("HxMessageBox");
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
                    if (window.Current.AutomationId == "CusMessageBox")
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
                Wait(200);
            }

            try
            {
                wp.Close();
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

        internal string EventDescription()
        {
            Search();
            bool find = false;
            for (int i = 0; i < Config.RedoCount; i++)
            {
                try
                {
                    WindowsUnit e = element.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.AutomationIdProperty, "pnlForm"));
                    e = e.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.AutomationIdProperty, "BodyBounds"));
                    e = e.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.AutomationIdProperty, "BodyClient"));
                    e = e.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.AutomationIdProperty, "pnlBodyRect"));

                    WindowsUnit doc = e.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.AutomationIdProperty, "rtbDescript"));
                    TextPattern tp = doc.GetCurrentPattern(TextPattern.Pattern) as TextPattern;
                    if (tp == null)
                    {
                        throw new UnSupportPatternException();
                    }
                    
                    string message = tp.DocumentRange.GetText(1024);
                    find = true;
                    return message;
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
                throw new NullControlException("航信对话框", "事件描述");
            }
            return null;
        }

        internal void Click(string buttonName)
        {
            if (string.IsNullOrEmpty(buttonName))
            {
                throw new InvalidParamValueException("Button name can not be none");
            }
            Search();
            WindowsUnit e = element.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.AutomationIdProperty, "pnlForm"));
            e = e.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.AutomationIdProperty, "BodyBounds"));
            e = e.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.AutomationIdProperty, "BodyClient"));
            e = e.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.AutomationIdProperty, "pnlBodyRect"));
            e = e.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.AutomationIdProperty, "panel1"));

            AndCondition condition = new AndCondition(new PropertyCondition(AutomationElement.NameProperty, buttonName), new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button));
            WindowsUnit btn = e.FindFirst(TreeScope.Children, condition);

            if (btn == null)
            {
                throw new NullControlException("航信对话框", buttonName);
            }

            InvokePattern invPattern = btn.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;

            if (invPattern == null)
            {
                throw new UnSupportPatternException();
            }

            //invPattern.Invoke();
            //Wait(1000);
            try
            {
                Robot.ExecuteWithTimeOut(() =>
                {
                    invPattern.Invoke();
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
                            invPattern.Invoke();
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
