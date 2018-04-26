namespace Mock.Tools.Controls
{
    using System;
    using System.Threading;
    /// <summary>
    /// 表示窗口的基本信息的对象
    /// 
    /// 2016.07.11 由 韩志强 生成
    /// 
    /// </summary>
    public class WindowBaseInfo
    {
        /// <summary>
        /// 窗口名称
        /// </summary>
        public string WindowName { get; set; }

        private bool busy = false;
        private string friendlyName = null;
        /// <summary>
        /// 窗口别名
        /// </summary>
        public string FriendlyName
        {
            get
            {
                while (busy)
                {
                    LogManager.DebugFormat("{0} is busy", WindowName);
                    Robot.Recess(100);
                }
                return friendlyName;
            }
        }

        private string aid = null;
        /// <summary>
        /// Automation ID
        /// </summary>
        public string AutomationId
        {
            get
            {
                return aid;
            }
            set
            {
                aid = value;
                new Thread(() =>
                    {
                        busy = true;
                        try
                        {
                            friendlyName = RobotContext.GetWindowFriendlyName(aid);
                        }
                        catch { }
                        busy = false;
                    }).Start();
            }
        }

        /// <summary>
        /// 类名称
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 窗口句柄
        /// </summary>
        public IntPtr Handle { get; set; }

        /// <summary>
        /// 打开时的时间
        /// </summary>
        public DateTime OpenTime { get; set; }

        /// <summary>
        /// 类型ID
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 关闭时的时间
        /// </summary>
        public DateTime CloseTime { get; set; }

        /// <summary>
        /// 运行时唯一ID
        /// </summary>
        public string RuntimeId { get; set; }

        public override bool Equals(object obj)
        {
            if(obj == null)
            {
                return false;
            }
            if (!(obj is WindowBaseInfo))
            {
                return false;
            }
            WindowBaseInfo target = obj as WindowBaseInfo;
            return Handle == target.Handle;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
