package Mock.Data;

import Mock.Exception.TaistException;

public class CanNotFindDataException extends TaistException {
	/**
	 * 
	 */
	private static final long serialVersionUID = 1476845347854588957L;

	public CanNotFindDataException(String message) {
		super(String.format("Can not find data because %s", message));
	}
}
