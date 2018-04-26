namespace Mock.Data.Exception
{
    /// <summary>
    /// 表示找不到文件的异常
    /// </summary>
    public class CanNotFindFileException : TaistException
    {
        /// <summary>
        /// 无法找到指定路径的文件时抛出此异常
        /// </summary>
        /// <param name="fileName">文件名（包含路径）</param>
        public CanNotFindFileException(string fileName) : base(string.Format("Can not find file named {0}",fileName)) { }
    }
}
