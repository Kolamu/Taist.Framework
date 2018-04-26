namespace Mock.Data.Exception
{
    public class CanNotFindFunctionException : TaistException
    {
        public CanNotFindFunctionException(string functionName, string className) : base(string.Format("Can not find function named {0} in {1} class.", functionName, className)) { }
    }
}
