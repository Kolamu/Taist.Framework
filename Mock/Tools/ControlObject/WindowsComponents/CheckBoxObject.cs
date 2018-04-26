namespace Mock.Tools.Controls
{
    using System;
    using System.Threading;
    using System.Diagnostics;
    using System.Windows.Automation;
    using Mock.Tools.Exception;
    internal class CheckBoxObject : WinObject
    {
        private Exception exp = null;
        private AutoResetEvent CompleteEvent = null;
        private bool success = false;

        internal CheckBoxObject(string windowName, string checkboxName)
        {
            _windowName = windowName;
            _elementName = checkboxName;
            WWindow.SearchWindow(windowName);
            GetElement(windowName, checkboxName);
            if (!element.Current.ControlType.Equals(ControlType.CheckBox))
            {
                element = element.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.CheckBox));
                if (element == null)
                {
                    throw new NullControlException(windowName, checkboxName);
                }
            }
        }

        public CheckBoxObject(WindowsUnit e)
        {
            element = e;
        }

        internal void Check()
        {
            CompleteEvent = new AutoResetEvent(false);
            exp = null;
            success = false;
            Thread t = new Thread(new ThreadStart(DoCheck));
            t.IsBackground = true;
            t.Start();
            CompleteEvent.WaitOne(5000);
            if (exp != null)
            {
                throw exp;
            }
            if (!success)
            {
                try
                {
                    t.Abort();
                }
                catch { }
                throw new TimeOutException(string.Format("Check {1} CheckBox in {0} Window", _windowName, _elementName));
            }
        }

        private void DoCheck()
        {
            try
            {
                if (!element.Current.IsKeyboardFocusable)
                {
                    throw new ControlUnableException(_windowName, _elementName);
                }
                TogglePattern togglePattern = element.GetCurrentPattern(TogglePattern.Pattern) as TogglePattern;

                if (togglePattern == null)
                {
                    throw new UnClickableException();
                }

                while (togglePattern.Current.ToggleState != ToggleState.On)
                {
                    togglePattern.Toggle();
                    Wait(100);
                }

                success = true;
                CompleteEvent.Set();
            }
            catch (Exception ex)
            {
                exp = ex;
            }
        }

        internal void UnCheck()
        {
            CompleteEvent = new AutoResetEvent(false);
            exp = null;
            success = false;
            Thread t = new Thread(new ThreadStart(DoUnCheck));
            t.IsBackground = true;
            t.Start();
            CompleteEvent.WaitOne(5000);
            
            if (exp != null)
            {
                throw exp;
            }
            if (!success)
            {
                try
                {
                    t.Abort();
                }
                catch { }
                throw new TimeOutException(string.Format("UnCheck {1} CheckBox in {0} Window", _windowName, _elementName));
            }
        }

        private void DoUnCheck()
        {
            try
            {
                if (!element.Current.IsKeyboardFocusable)
                {
                    throw new ControlUnableException(_windowName, _elementName);
                }
                TogglePattern togglePattern = element.GetCurrentPattern(TogglePattern.Pattern) as TogglePattern;

                if (togglePattern == null)
                {
                    throw new UnClickableException();
                }

                while (togglePattern.Current.ToggleState != ToggleState.Off)
                {
                    togglePattern.Toggle();
                    Wait(100);
                }

                success = true;
                CompleteEvent.Set();
            }
            catch (Exception ex)
            {
                exp = ex;
            }
        }

        internal void Indeterminate()
        {
            CompleteEvent = new AutoResetEvent(false);
            exp = null;
            success = false;
            Thread t = new Thread(new ThreadStart(DoIndeterminate));
            t.IsBackground = true;
            t.Start();
            CompleteEvent.WaitOne(5000);
            
            if (exp != null)
            {
                throw exp;
            }
            if (!success)
            {
                try
                {
                    t.Abort();
                }
                catch { }
                throw new TimeOutException(string.Format("UnCheck {1} CheckBox in {0} Window", _windowName, _elementName));
            }
        }

        private void DoIndeterminate()
        {
            try
            {
                if (!element.Current.IsKeyboardFocusable)
                {
                    throw new ControlUnableException(_windowName, _elementName);
                }
                TogglePattern togglePattern = element.GetCurrentPattern(TogglePattern.Pattern) as TogglePattern;

                if (togglePattern == null)
                {
                    throw new UnClickableException();
                }

                while (togglePattern.Current.ToggleState != ToggleState.Indeterminate)
                {
                    togglePattern.Toggle();
                    Wait(100);
                }

                success = true;
                CompleteEvent.Set();
            }
            catch (Exception ex)
            {
                exp = ex;
            }
        }
    }
}
