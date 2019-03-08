package Mock.Data.Internal;

import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;

final class NumberProperty extends TaistPropertyImpl {

	public <V> void setValue(Object instance, V value) throws CannotFindMethodException, MethodInvokeException {
		if(setMethod == null) {
			throw new CannotFindMethodException(String.format("set%s", getName()));
		}
		
		try {
			if(value == null) {
				setMethod.invoke(instance, 0);
			}
			else {
				setMethod.invoke(instance, castValue(value.toString(), setMethod.getParameterTypes()[0]));
			}
		} catch (IllegalAccessException e) {
			throw new MethodInvokeException(setMethod.getName(), "Method has no access");
		} catch (IllegalArgumentException e) {
			throw new MethodInvokeException(setMethod.getName(), "Arguments error " + e.getMessage());
		} catch (InvocationTargetException e) {
			throw new MethodInvokeException(setMethod.getName(), e.getMessage());
		}
	}
	
	private Object castValue(String value, Class<?> targetType) {
		if(targetType == int.class) {
			return Integer.valueOf(value);
		}
		else if(targetType == byte.class) {
			return Byte.valueOf(value);
		}
		else if(targetType == short.class) {
			return Short.valueOf(value);
		}
		else if(targetType == long.class) {
			return Long.valueOf(value);
		}
		else if(targetType == double.class) {
			return Double.valueOf(value);
		}
		else if(targetType == float.class) {
			return Float.valueOf(value);
		}
		else {
			try {
				Method method = targetType.getMethod("valueOf");
				return method.invoke(null, value);
			} catch (NoSuchMethodException e) {
				e.printStackTrace();
			} catch (SecurityException e) {
				e.printStackTrace();
			} catch (IllegalAccessException e) {
				e.printStackTrace();
			} catch (IllegalArgumentException e) {
				e.printStackTrace();
			} catch (InvocationTargetException e) {
				e.printStackTrace();
			}
			return targetType.cast(value);
		}
	}
}
