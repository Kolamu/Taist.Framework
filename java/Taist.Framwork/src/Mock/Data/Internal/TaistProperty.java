package Mock.Data.Internal;

public interface TaistProperty {
	String getName();
	String getFriendlyName();
	
	<E> void setValue(Object instance, E value) throws CannotFindMethodException, MethodInvokeException;
	<E> E getValue(Object instance) throws CannotFindMethodException, MethodInvokeException;
}
