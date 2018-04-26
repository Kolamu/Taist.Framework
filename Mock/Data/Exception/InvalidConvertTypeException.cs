namespace Mock.Data.Exception
{
    public class InvalidConvertTypeException : TaistException
    {
        public InvalidConvertTypeException() { }
        public InvalidConvertTypeException(string message) : base(string.Format( "Invalid convert type : {0}", message)) { }
    }
}
