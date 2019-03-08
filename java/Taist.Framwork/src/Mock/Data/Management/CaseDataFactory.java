package Mock.Data.Management;

public class CaseDataFactory extends TaistDataFactory {
	private CaseDataFactory() {
		super("Data\\CaseData");
	}
	
	private static CaseDataFactory factory = null;
	
	public static synchronized CaseDataFactory getInstance() {
		if(factory == null) {
			factory = new CaseDataFactory();
		}
		
		return factory;
	}
}
