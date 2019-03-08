package Mock.Data.Helpers;

import Mock.Data.LogType;

/**
 * 
 * Log helper class
 * 
 * @author Kolamu
 * @version 1.0.20180616
 *
 */
public class LogHelper {
	public static void Debug(String pattern, Object...args) {
		Manual(String.format(pattern, args), LogType.DEBUG);
	}
	
	public static void Manual(String message, LogType logType) {
		System.out.println(message);
	}
}
