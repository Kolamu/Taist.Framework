package Mock.Data.Internal;

import Mock.Data.Annotations.BusinessMethod;

public interface TaistMethod {
	String getName();
	String getFullName();
	TaistType getType();
	Object Invoke(Object instance, Object... args) throws MethodInvokeException;
	boolean isKeywords();
	BusinessMethod getKeywordsInformation();
	boolean isStatic();
}
