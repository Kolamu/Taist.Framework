namespace Mock.Data
{
    using System.Collections.Generic;
    using System.Xml;
    using System.Windows.Automation;

    using Mock.Data.Attributes;
    /// <summary>
    /// 表示对象库中对控件的描述结构对象
    /// </summary>
    public class ElementInfo
    {
        /// <summary>
        /// 构造新对象
        /// </summary>
        public ElementInfo()
        {
            this.id = string.Empty;
            this.FriendlyName = string.Empty;
            this.Name = string.Empty;
            this.Type = string.Empty;
            this.AutomationId = string.Empty;
            this.RelativePath = string.Empty;
            this.RelativePosition = string.Empty;
            this.Description = string.Empty;
            this.Inherit = false;
        }

        /// <summary>
        /// Id号
        /// </summary>
        [FieldProperty("id", true)]
        public string id
        {
            get;
            set;
        }

        /// <summary>
        /// 别名
        /// </summary>
        [FieldProperty("FriendlyName", true)]
        public string FriendlyName
        {
            get;
            set;
        }

        /// <summary>
        /// 继承属性
        /// </summary>
        [FieldProperty("Inherit", true)]
        public bool Inherit
        {
            get;
            set;
        }

        /// <summary>
        /// 实际名称
        /// </summary>
        [FieldProperty("Name", false)]
        public string Name
        {
            get;
            set;
        }
        /// <summary>
        /// 类型
        /// </summary>
        [FieldProperty("Type", false)]
        public string Type
        {
            get;
            set;
        }
        /// <summary>
        /// 控件唯一标识
        /// </summary>
        [FieldProperty("AutomationId", false)]
        public string AutomationId
        {
            get;
            set;
        }
        /// <summary>
        /// 控件相对窗口位置
        /// </summary>
        [FieldProperty("RelativePosition", false)]
        public string RelativePosition
        {
            get;
            set;
        }
        /// <summary>
        /// 控件相对窗口的路径
        /// </summary>
        [FieldProperty("RelativePath", false)]
        public string RelativePath
        {
            get;
            set;
        }

        /// <summary>
        /// 说明
        /// </summary>
        [FieldProperty("Description", false)]
        public string Description
        {
            get;
            set;
        }
    }
}
