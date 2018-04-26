namespace Mock.Data.Exception
{
    public class LoadModuleFailedException : TaistException
    {
        public LoadModuleFailedException(string moduleName) : base(string.Format("Load module named {0} failed", moduleName)) { }
    }
}
