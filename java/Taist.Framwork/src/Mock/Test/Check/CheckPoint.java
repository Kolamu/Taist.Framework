package Mock.Test.Check;

import java.util.List;
import java.util.Map;

import Mock.Data.TaistData;
import Mock.Data.Helpers.StringHelper;
import Mock.Data.Xml.XmlNode;
import Mock.Test.TestContext;

public class CheckPoint extends TaistData {
	
	private String dataBh = null;
	private List<CheckPointUnit> checkList = null;
	
	public String getDataBh() {
		if(dataBh == null) {
			return getBh();
		}
		return dataBh;
	}
	
	public void setDataBh(String dataBh) {
		this.dataBh = dataBh;
	}
	
	public CheckResult check() {
		AggregateResult result = new AggregateResult();
		for(CheckPointUnit unit : checkList) {
			result.add(unit.check());
		}
		return result;
	}
	
	@Override
	public void init() {
		Map<String, XmlNode> checkNodeList = getDynamicProperties();
		TestContext cache = new TestContext();
		cache.setBh(dataBh);
		cache.refresh();
		for(XmlNode checkNode : checkNodeList.values()) {
			checkList.add(CheckPointUnitFactory.getCheckPointUnit(checkNode, cache));
		}
		getWritebackFilePath();
	}
	
	private String getWritebackFilePath() {
		if (StringHelper.isNullOrEmpty(dataFilePath)) {
            return null;
        }
        else {
        	return "Data\\NewCheck\\" + dataFilePath.substring(dataFilePath.indexOf("CheckData") + 10);
        }
	}
}
