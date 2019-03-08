package Mock.Data.Management;

public class CheckDataFactory extends TaistDataFactory {
	private CheckDataFactory() {
		super("Data\\CheckData");
	}
	
	private static CheckDataFactory factory = null;
	
	public static synchronized CheckDataFactory getInstance() {
		if(factory == null) {
			factory = new CheckDataFactory();
		}
		
		return factory;
	}
}
