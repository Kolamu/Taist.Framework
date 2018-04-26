namespace Mock.Data
{
    using System;
    /// <summary>
    /// 表示一个测试结果对象
    /// </summary>
    public class Result
    {
        /// <summary>
        /// 构造一个测试结果对象的实例
        /// </summary>
        public Result()
        {
        }

        /// <summary>
        /// 测试项
        /// </summary>
        public string TestItem
        {
            get;
            set;
        }

        /// <summary>
        /// 测试项的简要描述
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// 测试时间
        /// </summary>
        public DateTime StartTime
        {
            get;
            set;
        }

        /// <summary>
        /// 测试结束时间
        /// </summary>
        public DateTime StopTime
        {
            get;
            set;
        }

        /// <summary>
        /// 测试人员姓名
        /// </summary>
        public string TesterName
        {
            get;
            set;
        }

        /// <summary>
        /// 测试数据对象
        /// </summary>
        public string Data
        {
            get;
            set;
        }

        /// <summary>
        /// 测试用例对象
        /// </summary>
        public TestCase TestCase
        {
            get;
            set;
        }

        
    }
}
