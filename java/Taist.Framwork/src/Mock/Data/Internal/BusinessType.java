package Mock.Data.Internal;

import Mock.Data.Annotations.BusinessClass;

class BusinessType extends AbstractTaistType {
	public BusinessType(Class<?> realType) {
		super(realType);
	}
	
	@Override
	public String getFriendlyName() {
		BusinessClass[] busAnnotations = realType.getAnnotationsByType(BusinessClass.class);
		if(busAnnotations != null && busAnnotations.length > 0) {
			return busAnnotations[0].Name();
		}
		else {
			return super.getFriendlyName();
		}
	}
}
