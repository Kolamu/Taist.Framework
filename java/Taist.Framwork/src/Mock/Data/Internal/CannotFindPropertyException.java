package Mock.Data.Internal;

import Mock.Exception.TaistException;

public class CannotFindPropertyException extends TaistException {
	/**
	 * 
	 */
	private static final long serialVersionUID = -4443315128603165908L;

	public CannotFindPropertyException(String propertyName) {
		super("Can not find property named" + propertyName);
	}
}
