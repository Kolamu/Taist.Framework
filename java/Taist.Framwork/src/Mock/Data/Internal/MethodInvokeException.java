package Mock.Data.Internal;

import Mock.Exception.TaistException;

public class MethodInvokeException extends TaistException {
	/**
	 * 
	 */
	private static final long serialVersionUID = -7825919835684190371L;

	public MethodInvokeException(String methodName, String message) {
		super("Invoke method named " + methodName + " occur exception : " + message);
	}
}
