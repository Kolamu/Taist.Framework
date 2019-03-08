package Mock.Data.Internal;

import Mock.Data.Annotations.DataClass;
import Mock.Data.Helpers.StringHelper;

class DataType extends AbstractTaistType {
	public DataType(Class<?> realType) {
		super(realType);
	}
	
	@Override
	public String getFriendlyName() {
		DataClass[] dataAnnotations = realType.getAnnotationsByType(DataClass.class);
		if(dataAnnotations != null && dataAnnotations.length > 0) {
			String friendlyName = dataAnnotations[0].Name();
			return StringHelper.isNullOrEmpty(friendlyName) ? 
					super.getFriendlyName() : friendlyName;
		}
		else {
			return super.getFriendlyName();
		}
	}
}
