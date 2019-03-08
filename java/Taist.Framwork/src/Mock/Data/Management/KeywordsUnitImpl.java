package Mock.Data.Management;

import Mock.Data.Internal.MethodInvokeException;
import Mock.Data.Internal.TaistMethod;

final class KeywordsUnitImpl implements KeywordsUnit {

	private String name = null;
	private TaistMethod method = null;

	public KeywordsUnitImpl(String name, TaistMethod method) {
		this.name = name;
		this.method = method;
	}
	
	public String getName() {
		return name;
	}
	
	public void Inovke() throws MethodInvokeException {
		if(method.isStatic()) {
			method.Invoke(null);
		} else {
			method.Invoke(method.getType().createInstance());
		}
	}
}
