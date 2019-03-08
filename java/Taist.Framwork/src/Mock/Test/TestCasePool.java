package Mock.Test;

import java.util.List;

import Mock.Config;
import Mock.Data.Helpers.StringHelper;
import Mock.Data.Helpers.XmlHelper;
import Mock.Data.Management.TestCaseFactory;
import Mock.Data.Xml.XmlDocument;
import Mock.Data.Xml.XmlNode;
import Mock.Data.Xml.XmlNodeList;
import Mock.Exception.XmlFormatErrorException;

public class TestCasePool {
	
	public static void run() {
		try {
			XmlDocument doc = XmlHelper.loadXml(Config.getTestCasePool());
			XmlNodeList nodelist = doc.selectNodes("//Bh");
			for(XmlNode node : nodelist) {
				run(node.getInnerText());
			}
		} catch (XmlFormatErrorException e) {
			e.printStackTrace();
		}
	}
	
	public static void run(String listString) {
		run(StringHelper.parseBhList(listString));
	}
	
	public static void run(List<String> bhlist) {
		for(String bh : bhlist) {
			try {
				TestCase test = TestCaseFactory.getInstance().getData(bh);
				test.run();
			}
			catch (Throwable e) {
				e.printStackTrace();
			}
		}
	}
}
