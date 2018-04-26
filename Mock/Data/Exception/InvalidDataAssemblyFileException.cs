namespace Mock.Data.Exception
{
    /// <summary>
    /// 数据定义程序集文件非法异常
    /// </summary>
    public class InvalidDataAssemblyFileException : TaistException
    {
        /// <summary>
        /// 构造数据定义程序集文件非法异常的新实例
        /// </summary>
        public InvalidDataAssemblyFileException(string fileName, System.Exception innerException) : base(string.Format("The data assembly file {0} is invalid.", fileName)) { }
    }
}
