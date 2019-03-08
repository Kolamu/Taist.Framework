package Mock.Data.Management;

import java.util.ArrayList;

import Mock.Data.Internal.TaistPackage;
import Mock.Data.Internal.TaistType;
import Mock.Exception.CanNotFindTypeException;

final class SourceFactory {
	private static KeywordsFactory keywords = null;
	private static DataClassFactory dataClass = null;
	private static ExternalClassFactory extClass = null;
	
	static KeywordsFactory getKeywordsFactory() {
		return keywords;
	}
	
	static DataClassFactory getDataClassFactory() {
		return dataClass;
	}
	
	static ExternalClassFactory getExternalClassFactory() {
		return extClass;
	}
	
	static {
		keywords = new KeywordsFactory();
		dataClass = new DataClassFactory();
		loadSource();
	}
	
	private SourceFactory() {
	}
	
	private synchronized static void loadSource() {
		TaistPackage pack = TaistPackage.getPackage(".\\Bin");
		extClass = new ExternalClassFactory(pack);
		ArrayList<String> nameList = pack.getClassNameList();
		for(String name : nameList) {
			try {
				TaistType type = pack.getTaistType(name);
				if(type.isDataType()) {
					dataClass.save(type);
				}
				else if(type.isBusinessType()) {
					keywords.save(type);
				}
			} catch (CanNotFindTypeException e) {
				e.printStackTrace();
			}
		}
	}
}
