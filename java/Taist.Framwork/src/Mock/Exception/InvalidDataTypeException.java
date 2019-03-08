package Mock.Exception;

public class InvalidDataTypeException extends TaistException {
	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;

	public InvalidDataTypeException(String typeName) {
		super("Invalid data type " + typeName);
	}
}
