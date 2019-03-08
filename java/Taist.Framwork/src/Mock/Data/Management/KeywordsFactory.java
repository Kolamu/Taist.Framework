package Mock.Data.Management;

import java.util.HashMap;
import java.util.List;
import java.util.Map;

import Mock.Config;
import Mock.Data.CanNotFindDataException;
import Mock.Data.Annotations.BusinessMethod;
import Mock.Data.Internal.MethodInvokeException;
import Mock.Data.Internal.TaistMethod;
import Mock.Data.Internal.TaistType;

public final class KeywordsFactory {
	private Map<String, KeywordsUnit> keywordsMap = null;
	KeywordsFactory() {
		keywordsMap = new HashMap<String, KeywordsUnit>();
	}
	
	public static KeywordsFactory getInstance() {
		return SourceFactory.getKeywordsFactory();
	}
	
	void save(TaistType type) {
		List<TaistMethod> methodList = type.getMethods();
		
		for(TaistMethod method : methodList) {
			if(method.isKeywords()) {
				String keywordsString = getKeywordsString(method.getKeywordsInformation());
				if(keywordsMap.containsKey(keywordsString)) {
					continue;
				}
				else {
					keywordsMap.put(keywordsString, new KeywordsUnitImpl(keywordsString, method));
				}
			}
		}
	}
	
	public void Invoke(String keywordsString) throws CanNotFindDataException, MethodInvokeException {
		if(keywordsMap.containsKey(keywordsString)) {
			keywordsMap.get(keywordsString).Inovke();
		}
		else {
			throw new CanNotFindDataException("Keywords[" + keywordsString + "] is not exist.");
		}
	}
	
	private String getKeywordsString(BusinessMethod busMethod) {
		String keywords = null;
		keywords = busMethod.Keywords() + "," + busMethod.SubKeywords() + ",";
		if(busMethod.TargetProject().isEmpty()) {
			keywords = keywords + Config.getTargetProject();
		}
		else {
			keywords = keywords + busMethod.TargetProject();
		}
		return keywords;
	}
}
