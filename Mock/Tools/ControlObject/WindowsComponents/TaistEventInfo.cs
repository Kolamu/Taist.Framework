namespace Mock.Tools.Controls
{
    using System;
    /// <summary>
    /// 自动化测试事件目标信息
    /// </summary>
    public class TaistControlEventInfo
    {
        public string WindowName { get; set; }
        public string ControlName { get; set; }
        public string Resverd1 { get; set; }
        public string Resverd2 { get; set; }
        public string Resverd3 { get; set; }
        public string Resverd4 { get; set; }
        public TaistControlType ControlType { get; set; }
        public TaistEventType EventType { get; set; }
        public DateTime EventTime { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is TaistControlEventInfo)
            {
                TaistControlEventInfo EventInfo = (TaistControlEventInfo)obj;
                return string.Equals(EventInfo.WindowName, this.WindowName)
                                && string.Equals(EventInfo.ControlName, this.ControlName)
                                && string.Equals(EventInfo.Resverd1, this.Resverd1)
                                && string.Equals(EventInfo.Resverd2, this.Resverd2)
                                && string.Equals(EventInfo.Resverd3, this.Resverd3)
                                && string.Equals(EventInfo.Resverd4, this.Resverd4)
                                && EventInfo.ControlType == this.ControlType
                                && EventInfo.EventType == this.EventType;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    /// <summary>
    /// 自动化测试控件类型
    /// </summary>
    public enum TaistControlType
    {
        Button,
        ToolBar,
        Edit,
        Combox,
        PopMenu,
        Menu,
    }

    /// <summary>
    /// 自动化测试事件类型
    /// </summary>
    public enum TaistEventType
    {
        Click,
        Input
    }

    /// <summary>
    /// 自动化测试框架激活控件事件处理
    /// </summary>
    /// <param name="eventInfo"></param>
    public delegate void TaistControlEventHandler(TaistControlEventInfo eventInfo);
}
