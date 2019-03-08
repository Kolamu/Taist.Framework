package Mock.Exception;

public class CanNotFindTypeException extends TaistException {
	/**
	 * 
	 */
	private static final long serialVersionUID = -7577855130507654466L;

	public CanNotFindTypeException(String typeName) {
		super("Can not find class named " + typeName);
	}
}
