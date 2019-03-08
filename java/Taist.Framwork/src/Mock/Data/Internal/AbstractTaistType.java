package Mock.Data.Internal;

import java.lang.reflect.Constructor;
import java.lang.reflect.Method;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import Mock.Data.TaistData;
import Mock.Data.Annotations.BusinessClass;
import Mock.Data.Annotations.DataClass;
import Mock.Data.Annotations.TypeMethod;
import Mock.Data.Helpers.StringHelper;
import Mock.Data.Helpers.XmlHelper;
import Mock.Data.Xml.XmlNode;

public abstract class AbstractTaistType implements TaistType {
	protected Class<?> realType = null;
	
	private Map<String, TaistMethod> methodMap = null;
	private Map<String, TaistProperty> propertyMap = null;
	
	public AbstractTaistType(Class<?> realType) {
		this.realType = realType;
		parseType();
	}
	
	public String getName() {
		return realType.getSimpleName();
	}
	
	public String getFriendlyName() {
		return getName();
	}
	
	public String getFullName() {
		return realType.getName();
	}
	
	public TaistProperty getProperty(String propertyName) {
		if(StringHelper.isNullOrEmpty(propertyName)) {
			System.out.println("propertyName is null");
			return null;
		}
		
		if(propertyMap == null) {
			System.out.println("propertyMap is null");
			return null;
		}
		
		if(propertyMap.containsKey(propertyName)) {
			return propertyMap.get(propertyName);
		}
		
		if(propertyMap.containsKey(propertyName.toLowerCase())) {
			return propertyMap.get(propertyName.toLowerCase());
		}
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
		} catch (Exception e) {
			e.printStackTrace();
		}
		return null;
	}
	
	public Object createInstance(Object...objects) {
		Constructor<?> typeConstructor;
		
		try {
			typeConstructor = realType.getConstructor(getParameterTypes(objects));
			return typeConstructor.newInstance(objects);
		} catch (Exception e) {
			e.printStackTrace();
		}
		return null;
	}
	
	public Object createInstance(XmlNode node) {
		try {
			Object instance = XmlHelper.ToObject(node, realType);
			if(instance instanceof TaistData) {
				TaistData data = (TaistData)instance;
				data.init();
			}
			return instance;
		} catch (Exception e) {
			e.printStackTrace();
		}
		return null;
	}
	
	public boolean isDataType() {
		return realType.isAnnotationPresent(DataClass.class)
				|| extendsOf(TaistData.class);
	}
	
	private boolean extendsOf(Class<?> type) {
		if(type == null) {
			return false;
		}
		
		Class<?> parent = realType;
		while(parent != Object.class) {
			if(parent == type) {
				return true;
			}
			parent = parent.getSuperclass();
		}
		
		return false;
	}
	
	private Class<?>[] getParameterTypes(Object[] parameterArray){
		Class<?>[] paramTypeArray = new Class<?>[parameterArray.length];
		for(int i=0;i<parameterArray.length;i++) {
			paramTypeArray[i] = parameterArray[i].getClass();
		}
		return paramTypeArray;
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
		if(ignoreMethod(name)) {
			return;
		}
		String propertyName = name.substring(3);
		if(propertyMap == null) {
			propertyMap = new HashMap<String, TaistProperty>();
		}
		if(!propertyMap.containsKey(propertyName)) {
			TaistProperty property = createProperty(method);
			propertyMap.put(propertyName, property);
			if(!StringHelper.equals(propertyName, propertyName.toLowerCase())) {
				propertyMap.put(propertyName.toLowerCase(), property);
			}
		}
		
		if(name.startsWith("set")) {
			((TaistPropertyImpl)propertyMap.get(propertyName)).setSetMethod(method);
		}
		else if(name.startsWith("get")) {
			((TaistPropertyImpl)propertyMap.get(propertyName)).setGetMethod(method);
		}
	}
	
	private boolean ignoreMethod(String methodName) {
		return IGNORE_METHOD_NAME_LIST.contains(methodName);
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
	
	private final List<String> IGNORE_METHOD_NAME_LIST = Arrays.asList(
		"getClass",
		"getDynamicProperty",
		"getDynamicProperties"
	);
}
