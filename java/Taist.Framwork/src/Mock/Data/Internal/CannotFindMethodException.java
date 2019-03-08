package Mock.Data.Internal;

import Mock.Exception.TaistException;

public class CannotFindMethodException extends TaistException {
	/**
	 * 
	 */
	private static final long serialVersionUID = -1993360175910975943L;

	public CannotFindMethodException(String methodName) {
		super("Can not find method named " + methodName);
	}
}
