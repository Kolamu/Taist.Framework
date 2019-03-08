package Mock.Data.Internal;

import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;
import java.lang.reflect.Modifier;

import Mock.Data.Annotations.BusinessMethod;

final class TaistMethodImpl implements TaistMethod {
	protected Method realMethod = null; 
	private TaistType type = null;
	
	public TaistMethodImpl(Method realMethod, TaistType type) {
		this.realMethod = realMethod;
		this.type = null;
	}
	
	public String getName() {
		return realMethod.getName();
	}
	
	public String getFullName() {
		Class<?>[] argTypeArray = realMethod.getParameterTypes();
		String fullName = getName();
		for(Class<?> argType : argTypeArray) {
			fullName += "," + argType.getName();
		}
		return fullName;
	}
	
	public TaistType getType() {
		return type;
	}
	
	public Object Invoke(Object instance, Object... args) throws MethodInvokeException {
		try {
			return realMethod.invoke(instance, args);
		} catch (IllegalAccessException e) {
			throw new MethodInvokeException(getName(), "Method has no access");
		} catch (IllegalArgumentException e) {
			throw new MethodInvokeException(getName(), "Arguments error " + e.getMessage());
		} catch (InvocationTargetException e) {
			throw new MethodInvokeException(getName(), e.getMessage());
		}
	}
	
	public boolean isKeywords() {
		return realMethod.isAnnotationPresent(BusinessMethod.class);
	}
	
	public Method getJavaMethod() {
		return realMethod;
	}
	
	public boolean isStatic() {
		return Modifier.isStatic(realMethod.getModifiers());
	}

	public BusinessMethod getKeywordsInformation() {
		if(isKeywords()) {
			return realMethod.getAnnotationsByType(BusinessMethod.class)[0];
		} else {
			return null;
		}
	}
}
