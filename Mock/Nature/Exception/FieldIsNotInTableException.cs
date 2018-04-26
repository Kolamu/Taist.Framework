namespace Mock.Nature.Exception
{
    /// <summary>
    /// Exception when set field to table which is not contains the named field occur.
    /// </summary>
    public class FieldIsNotInTableException : TaistException
    {
        /// <summary>
        /// Create an instance of FieldIsNotInTableException type.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="fieldName"></param>
        public FieldIsNotInTableException(string tableName, string fieldName) : base(string.Format("Field named {1} is not a member of table named {0}.", tableName, fieldName)) { }
    }
}
