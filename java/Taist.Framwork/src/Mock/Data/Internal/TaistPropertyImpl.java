package Mock.Data.Internal;

import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;

import Mock.Data.Annotations.FieldProperty;

public abstract class TaistPropertyImpl implements TaistProperty {
	protected Method getMethod = null;
	protected Method setMethod = null;
	private String propertyName = "";
	private String friendlyName = "";
	
	void setSetMethod(Method setMethod) {
		this.setMethod = setMethod;
		if(setMethod != null) {
			propertyName = setMethod.getName().substring(3);
		}
		
		if(setMethod.isAnnotationPresent(FieldProperty.class)) {
			friendlyName = setMethod.getAnnotationsByType(FieldProperty.class)[0].Name();
		}
	}
	
	void setGetMethod(Method getMethod) {
		this.getMethod = getMethod;
		if(getMethod != null) {
			propertyName = getMethod.getName().substring(3);
		}
		
		if(getMethod.isAnnotationPresent(FieldProperty.class)) {
			friendlyName = getMethod.getAnnotationsByType(FieldProperty.class)[0].Name();
		}
	}
	
	public String getName() {
		return propertyName;
	}
	
	public String getFriendlyName() {
		if(friendlyName.isEmpty()) {
			return propertyName;
		}
		
		return friendlyName;
	}
	
	@SuppressWarnings("unchecked")
	public <V> V getValue(Object instance) throws CannotFindMethodException, MethodInvokeException {
		if(getMethod == null) {
			throw new CannotFindMethodException(String.format("get%s", propertyName));
		}
		
		try {
			return (V)getMethod.invoke(instance);
		} catch (IllegalAccessException e) {
			throw new MethodInvokeException(getName(), "Method has no access");
		} catch (IllegalArgumentException e) {
			throw new MethodInvokeException(getName(), "Arguments error " + e.getMessage());
		} catch (InvocationTargetException e) {
			throw new MethodInvokeException(getName(), e.getMessage());
		}
	}
}
