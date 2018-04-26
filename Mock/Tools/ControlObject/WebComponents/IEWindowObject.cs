namespace Mock.Tools.Controls
{
    using System;
    using System.Threading;
    using System.Windows.Automation;

    using Mock.Tools.Exception;
    internal class IEWindowObject : WebObject
    {
        private bool timeout = false;
        internal IEWindowObject(string windowName)
        {
            _windowName = windowName;
        }

        internal void Search()
        {
            if (string.Equals(_windowName, _CurrentWindowName))
            {
                try
                {
                    GetElement(_windowName);
                    return;
                }
                catch (NullControlException) { }
                catch (WindowIsInactiveException) { return; }
            }
            timeout = false;
            Timer timer = new Timer(Tick, null, 60000, 1000);

            while (!Find())
            {
                if (timeout)
                {
                    timer.Dispose();
                    throw new TimeOutException(string.Format("Search {0}", _windowName));
                }
                Robot.Recess(1000);
            }
            //timer.Dispose();
            try
            {
                timer.Dispose();
            }
            catch { }
        }

        private void Tick(object sender)
        {
            timeout = true;
        }

        internal bool Find()
        {
            try
            {
                GetElement(_windowName);
                return true;
            }
            catch (WindowIsInactiveException)
            {
                return true;
            }
            catch (NullControlException)
            {
                return false;
            }
            catch (WarningWindowExistException)
            {
                return true;
            }
            //else
            //{
            //    throw ex;
            //}
        }

        internal void SetMaximize()
        {
            Search();
            WindowsUnit ieWindow = RobotContext.InternetExploreWindow;
            if (!ieWindow.Current.IsEnabled)
            {
                throw new ControlUnableException(_windowName);
            }
            WindowPattern windowPattern = ieWindow.GetCurrentPattern(WindowPattern.Pattern) as WindowPattern;
            if (windowPattern == null)
            {
                throw new UnSupportPatternException();
            }
            if (!windowPattern.Current.CanMaximize)
            {
                throw new WindowCanNotMaximizeException();
            }

            windowPattern.SetWindowVisualState(WindowVisualState.Maximized);
        }

        internal void SetTopMost()
        {
            Search();
            WindowsUnit ieWindow = RobotContext.InternetExploreWindow;
            if (!ieWindow.Current.IsEnabled)
            {
                throw new ControlUnableException(_windowName);
            }
            WindowPattern windowPattern = ieWindow.GetCurrentPattern(WindowPattern.Pattern) as WindowPattern;
            if (windowPattern == null)
            {
                throw new UnSupportPatternException();
            }
            if (windowPattern.Current.CanMaximize)
            {
                windowPattern.SetWindowVisualState(WindowVisualState.Maximized);
            }
            else
            {
                windowPattern.SetWindowVisualState(WindowVisualState.Normal);
            }
        }
    }
}
