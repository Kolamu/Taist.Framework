namespace Mock.Nature.Exception
{
    /// <summary>
    /// Exception when data base has not been opened occur.
    /// </summary>
    public class DataBaseNotOpenException : TaistException
    {
        /// <summary>
        /// Create an instance of DataBaseNotOpenException type.
        /// </summary>
        /// <param name="dbName"></param>
        public DataBaseNotOpenException(string dbName) : base(string.Format("Database named {0} has not open.", dbName)) { }
    }
}
