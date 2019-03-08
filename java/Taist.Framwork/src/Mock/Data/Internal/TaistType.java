package Mock.Data.Internal;

import java.util.List;

import Mock.Data.Xml.XmlNode;

public interface TaistType {
	String getName();
	String getFriendlyName();
	String getFullName();
	Object createInstance();
	Object createInstance(Object...objects);
	Object createInstance(XmlNode node);
	TaistProperty getProperty(String propertyName);
	TaistMethod getMethod(String methodName);
	List<TaistMethod> getMethods();
	List<TaistProperty> getProperties();
	boolean isBusinessType();
	boolean isDataType();
}
