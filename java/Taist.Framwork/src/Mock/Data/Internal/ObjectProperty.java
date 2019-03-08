package Mock.Data.Internal;

import java.lang.reflect.InvocationTargetException;

final class ObjectProperty extends TaistPropertyImpl {
	public <V> void setValue(Object instance, V value) throws CannotFindMethodException, MethodInvokeException {
		if(setMethod == null) {
			throw new CannotFindMethodException(String.format("set%s", getName()));
		}
		
		try {
			setMethod.invoke(instance, value);
		} catch (IllegalAccessException e) {
			throw new MethodInvokeException(getName(), "Method has no access");
		} catch (IllegalArgumentException e) {
			throw new MethodInvokeException(getName(), "Arguments error " + e.getMessage());
		} catch (InvocationTargetException e) {
			throw new MethodInvokeException(getName(), e.getMessage());
		}
	}
}
