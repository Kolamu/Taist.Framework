package Mock.Data.Internal;

import java.lang.reflect.InvocationTargetException;

final class StringProperty extends TaistPropertyImpl {

	@Override
	public <E> void setValue(Object instance, E value) throws CannotFindMethodException, MethodInvokeException {
		if(setMethod == null) {
			throw new CannotFindMethodException(String.format("set%s", getName()));
		}
		
		try {
			if(value == null) {
				setMethod.invoke(instance, new Object[] { null });
			}
			else {
				setMethod.invoke(instance, value.toString());
			}
		} catch (IllegalAccessException e) {
			throw new MethodInvokeException(getName(), "Method has no access");
		} catch (IllegalArgumentException e) {
			throw new MethodInvokeException(getName(), "Arguments error " + e.getMessage());
		} catch (InvocationTargetException e) {
			throw new MethodInvokeException(getName(), e.getMessage());
		}
	}

}
