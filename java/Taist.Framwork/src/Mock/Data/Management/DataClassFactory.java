package Mock.Data.Management;

import java.util.HashMap;
import java.util.Map;

import Mock.Data.Helpers.StringHelper;
import Mock.Data.Internal.TaistType;
import Mock.Exception.CanNotFindTypeException;

public final class DataClassFactory {
	Map<String, TaistType> nameTypeMap = null;
	
	DataClassFactory() {
		nameTypeMap = new HashMap<String, TaistType>();
	}
	
	public static DataClassFactory getInstance() {
		return SourceFactory.getDataClassFactory();
	}
	
	void save(TaistType type) {
		String name = type.getName();
		String friendlyName = type.getFriendlyName();
		
		if(StringHelper.equals(name, friendlyName)) {
			putTypeToMap(friendlyName, type);
		}
		
		putTypeToMap(name, type);
		putTypeToMap(type.getFullName(), type);
	}
	
	public TaistType getType(String typeName) throws CanNotFindTypeException {
		if(nameTypeMap.containsKey(typeName)) {
			return nameTypeMap.get(typeName);
		}
		else {
			throw new CanNotFindTypeException(typeName);
		}
	}
	
	private void putTypeToMap(String name, TaistType type) {
		if(!nameTypeMap.containsKey(name)) {
			nameTypeMap.put(name, type);
		}
	}
}
