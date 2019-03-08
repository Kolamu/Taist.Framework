package Mock.Test;

import java.util.LinkedList;
import java.util.List;
import java.util.Map;

import Mock.Data.TaistData;
import Mock.Data.Helpers.DataHelper;
import Mock.Data.Xml.XmlNode;

public final class TestCase extends TaistData {
	private int id = 0;
	private String name = null;
	private String description = null;
	private List<Step> stepList = null;
	
	public int getId() {
		return id;
	}

	public void setId(int id) {
		this.id = id;
	}

	public String getName() {
		return name;
	}

	public void setName(String name) {
		this.name = name;
	}

	public String getDescription() {
		return description;
	}

	public void setDescription(String description) {
		this.description = description;
	}
	
	public void run() {
		for(Step step : stepList) {
			step.run();
		}
	}
	
	@Override
	public void init() {
		Map<String, XmlNode> dataMap = getDynamicProperties();
		stepList = new LinkedList<Step>();
		for(String name : dataMap.keySet()) {
			if(name.startsWith("Step")) {
				try {
					Step step = DataHelper.GetData(dataMap.get(name));
					stepList.add(step);
				} catch (Throwable e) {
					e.printStackTrace();
				}
				
			}
		}
	}
}
