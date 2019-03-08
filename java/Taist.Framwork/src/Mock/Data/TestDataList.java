package Mock.Data;

import java.util.ArrayList;

public class TestDataList<E>{
	ArrayList<E> storage = null;
	
	public TestDataList(){
		
	}
	
	public TestDataList(String testDataBh){
		
	}
	
	public void Add(E testData) {
		if(storage == null) {
			storage = new ArrayList<>();
		}
		
		storage.add(testData);
	}
	
	
}