package Mock.Data.Internal;

import java.lang.reflect.Constructor;
import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import Mock.Data.Annotations.BusinessClass;
import Mock.Data.Annotations.DataClass;
import Mock.Data.Annotations.TypeMethod;

public abstract class TaistTypeImpl implements TaistType {
	protected Class<?> realType = null;
	
	private Map<String, TaistMethod> methodMap = null;
	private Map<String, TaistProperty> propertyMap = null;
	
	public TaistTypeImpl(Class<?> realType) {
		this.realType = realType;
		parseType();
	}
	
	public String getName() {
		return realType.getName();
	}
	
	public String getFriendlyName() {
		return getName();
	}
	
	public String getFullName() {
		return realType.getPackage().getName() + "." + realType.getName();
	}
	
	public TaistProperty getProperty(String propertyName) {
		//Field 
		return null;
	}
	
	public TaistMethod getMethod(String methodName) {
		try {
			return new TaistMethodImpl(realType.getMethod(methodName), this);
		} catch (NoSuchMethodException e) {
			return null;
		} catch (SecurityException e) {
			return null;
		}
	}
	
	public List<TaistMethod> getMethods(){
		return new ArrayList<TaistMethod>(methodMap.values());
	}
	
	public List<TaistProperty> getProperties(){
		return new ArrayList<TaistProperty>(propertyMap.values());
	}
	
	public boolean isBusinessType() {
		return realType.isAnnotationPresent(BusinessClass.class);
	}
	
	public Object createInstance() {
		Constructor<?> typeConstructor;
		try {
			typeConstructor = realType.getConstructor();
			return typeConstructor.newInstance();
		} catch (NoSuchMethodException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (SecurityException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (InstantiationException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (IllegalAccessException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (IllegalArgumentException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (InvocationTargetException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		return null;
	}
	
	public boolean isDataType() {
		return realType.isAnnotationPresent(DataClass.class);
	}
	
	private void parseType() {
		Method[] methodArray = realType.getMethods();
		for(Method method : methodArray) {
			String name = method.getName();
			if(name.startsWith("set")
					&& !method.isAnnotationPresent(TypeMethod.class)
					&& method.getParameterTypes().length == 1) {
				addProperty(method);
			}
			else if(name.startsWith("get")
					&& !method.isAnnotationPresent(TypeMethod.class)
					&& method.getParameterTypes().length == 0) {
				addProperty(method);
			}
			else {
				addMethod(method);
			}
		}
	}
	
	private void addProperty(Method method) {
		String name = method.getName();
		String propertyName = name.substring(3);
		if(propertyMap == null) {
			propertyMap = new HashMap<String, TaistProperty>();
		}
		if(!propertyMap.containsKey(propertyName)) {
			TaistProperty property = createProperty(method);
			propertyMap.put(propertyName, property);
		}
		
		if(name.startsWith("set")) {
			((TaistPropertyImpl)propertyMap.get(propertyName)).setSetMethod(method);
		}
		else if(name.startsWith("get")) {
			((TaistPropertyImpl)propertyMap.get(propertyName)).setGetMethod(method);
		}
	}
	
	private void addMethod(Method method) {
		TaistMethod methodToAdd = new TaistMethodImpl(method, this);
		String methodName = methodToAdd.getName();
		if(methodMap == null) {
			methodMap = new HashMap<String, TaistMethod>();
		}
		if(methodMap.containsKey(methodName)) {
			TaistMethod methodToRestore = methodMap.get(methodName);
			if(!methodToRestore.getFullName().equals(methodToRestore.getName())) {
				methodMap.put(methodToRestore.getFullName(), methodToRestore);
			}
			methodMap.put(methodToAdd.getFullName(), methodToAdd);
		}
		else {
			methodMap.put(methodName, methodToAdd);
		}
	}
	
	private TaistProperty createProperty(Method method) {
		String methodName = method.getName();
		Class<?> propertyType = null;
		if(methodName.startsWith("get")) {
			propertyType = method.getReturnType();
		}
		else if(methodName.startsWith("set")) {
			propertyType = method.getParameterTypes()[0];
		}
		
		if(propertyType == String.class) {
			return new StringProperty();
		}
		else if(propertyType == int.class){
			return new NumberProperty();
		}
		else {
			return new ObjectProperty();
		}
	}
}
