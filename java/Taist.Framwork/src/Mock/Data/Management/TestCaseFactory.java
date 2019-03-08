package Mock.Data.Management;

public class TestCaseFactory extends TaistDataFactory {
	private TestCaseFactory() {
		super("Data\\TestCase");
	}
	
	private static TestCaseFactory factory = null;
	
	public static synchronized TestCaseFactory getInstance() {
		if(factory == null) {
			factory = new TestCaseFactory();
		}
		
		return factory;
	}
}
