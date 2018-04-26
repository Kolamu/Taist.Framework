namespace Mock.Tools.Controls
{
    using Mock.Nature.Native;
    using Mock.Tools.Exception;

    using System;
    using System.Collections.Generic;
    using System.Windows.Automation;
    public class WindowsUnit
    {
        public static implicit operator WindowsUnit(AutomationElement element)
        {
            if (element == null) return null;
            WindowsUnit unit = new WindowsUnit();
            unit.element = element;
            unit.current = new CurrentInfomation(element);
            return unit;
        }

        public static implicit operator AutomationElement(WindowsUnit unit)
        {
            if (unit == null) return null;
            return unit.element;
        }

        private UnitInfomation current = null;
        public UnitInfomation Current { get { return current; } }

        private AutomationElement element = null;

        public int[] GetRuntimeId()
        {
            try
            {
                return element.GetRuntimeId();
            }
            catch
            {
                return new int[] { -1, -1 };
            }
        }

        internal object GetCurrentPattern(AutomationPattern pattern)
        {
            try
            {
                TestElement(element.Current.ClassName);
            }
            catch
            {
                throw new ControlUnableException(RobotContext.WindowName, RobotContext.ElementName);
            }
            return element.GetCurrentPattern(pattern);
        }

        private void TestElement(object obj) { }

        public WindowsUnit FindFirst(TreeScope scope, Condition condition)
        {
            try
            {
                return element.FindFirst(scope, condition);
            }
            catch
            {
                throw new ControlUnableException(RobotContext.WindowName, RobotContext.ElementName);
            }
        }

        public AutomationElementCollection FindAll(TreeScope scope, Condition condition)
        {
            try
            {
                return element.FindAll(scope, condition);
            }
            catch
            {
                throw new ControlUnableException(RobotContext.WindowName, RobotContext.ElementName);
            }
        }

        public static WindowsUnit FromHandle(int windowHandle)
        {
            return AutomationElement.FromHandle((IntPtr)windowHandle);
        }

        public static WindowsUnit FromHandle(IntPtr windowHandle)
        {
            return AutomationElement.FromHandle(windowHandle);
        }

        internal bool SetFocus()
        {
            for (int i = 0; i < Config.RedoCount; i++)
            {
                try
                {
                    element.SetFocus();
                    return true;
                }
                catch
                {
                }
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            try
            {
                if (obj is WindowsUnit)
                {
                    return element.Equals(((WindowsUnit)obj).element);
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            try
            {
                return element.GetHashCode();
            }
            catch
            {
                throw new ControlUnableException(RobotContext.WindowName, RobotContext.ElementName);
            }
        }
    }

    public interface UnitInfomation
    {
        string Name { get; }
        string ClassName { get; }
        string AutomationId { get; }
        ControlType ControlType { get; }
        int NativeWindowHandle { get; }
        bool IsEnabled { get; }
        System.Windows.Rect BoundingRectangle { get; }
        bool IsKeyboardFocusable { get; }
        bool IsOffscreen { get; }
        int ProcessId { get; }
    }

    internal class CurrentInfomation : UnitInfomation
    {
        private AutomationElement element = null;
        internal CurrentInfomation(AutomationElement element)
        {
            this.element = element;
        }

        public string Name
        {
            get
            {
                for (int i = 0; i < Config.RedoCount; i++)
                {
                    try
                    {
                        return element.Current.Name;
                    }
                    catch (Exception)
                    {
                        Robot.Recess(100);
                    }
                }
                return "WindowsUnit.Name";
            }
        }
        public string ClassName
        {
            get
            {
                for (int i = 0; i < Config.RedoCount; i++)
                {
                    try
                    {
                        return element.Current.ClassName;
                    }
                    catch(Exception)
                    {
                        Robot.Recess(100);
                    }
                }
                return ("WindowsUnit.ClassName");
            }
        }
        public string AutomationId
        {
            get
            {
                for (int i = 0; i < Config.RedoCount; i++)
                {
                    try
                    {
                        return element.Current.AutomationId;
                    }
                    catch (Exception)
                    {
                        Robot.Recess(100);
                    }
                }
                return ("WindowsUnit.AutomationId");
            }
        }

        public ControlType ControlType
        {
            get
            {
                for (int i = 0; i < Config.RedoCount; i++)
                {
                    try
                    {
                        return element.Current.ControlType;
                    }
                    catch (Exception)
                    {
                        Robot.Recess(100);
                    }
                }
                return ControlType.Pane;
            }
        }
        public int NativeWindowHandle
        {
            get
            {
                for (int i = 0; i < Config.RedoCount; i++)
                {
                    try
                    {
                        return element.Current.NativeWindowHandle;
                    }
                    catch (Exception)
                    {
                        Robot.Recess(100);
                    }
                }
                return -1;
            }
        }

        public bool IsEnabled
        {
            get
            {
                for (int i = 0; i < Config.RedoCount; i++)
                {
                    try
                    {
                        return element.Current.IsEnabled;
                    }
                    catch (Exception ex)
                    {
                        LogManager.ErrorOnlyPrint(ex);
                        Robot.Recess(100);
                    }
                }
                return false;
            }
        }
        public System.Windows.Rect BoundingRectangle
        {
            get
            {
                for (int i = 0; i < Config.RedoCount; i++)
                {
                    try
                    {
                        return element.Current.BoundingRectangle;
                    }
                    catch (Exception)
                    {
                        Robot.Recess(100);
                    }
                }
                return new System.Windows.Rect();
            }
        }
        public bool IsKeyboardFocusable
        {
            get
            {
                for (int i = 0; i < Config.RedoCount; i++)
                {
                    try
                    {
                        return element.Current.IsKeyboardFocusable;
                    }
                    catch (Exception)
                    {
                        Robot.Recess(100);
                    }
                }
                return false;
            }
        }
        public bool IsOffscreen
        {
            get
            {
                for (int i = 0; i < Config.RedoCount; i++)
                {
                    try
                    {
                        return element.Current.IsOffscreen;
                    }
                    catch (Exception)
                    {
                        Robot.Recess(100);
                    }
                }
                return true;
            }
        }
        public int ProcessId
        {
            get
            {
                for (int i = 0; i < Config.RedoCount; i++)
                {
                    try
                    {
                        return element.Current.ProcessId;
                    }
                    catch (Exception)
                    {
                        Robot.Recess(100);
                    }
                }
                return -1;
            }
        }
        
    }
}
