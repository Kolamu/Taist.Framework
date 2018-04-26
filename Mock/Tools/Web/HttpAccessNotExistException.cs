namespace Mock.Tools.Web
{
    public class HttpAccessNotExistException : TaistException
    {
        public HttpAccessNotExistException(string name) : base(string.Format("HttpAccess named {0} is not exist", name)) { }
    }
}
