package Mock.Data.Management;

import Mock.Data.Internal.TaistPackage;
import Mock.Data.Internal.TaistType;
import Mock.Exception.CanNotFindTypeException;

public final class ExternalClassFactory {
	TaistPackage pack = null;
	
	ExternalClassFactory(TaistPackage pack) {
		this.pack = pack;
	}
	
	public static ExternalClassFactory getInstance() {
		return SourceFactory.getExternalClassFactory();
	}
	
	public TaistType getType(String typeName) throws CanNotFindTypeException {
		return pack.getTaistType(typeName);
	}
}
