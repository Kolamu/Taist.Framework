package taist.global.test;

import Mock.Data.Helpers.XmlHelper;
import Mock.Data.Xml.XmlDocument;
import Mock.Data.Xml.XmlNode;

public class Executor {
	
	private String user;
	
	public String getUser() {
		return user;
	}
	
	public void setUser(String user) {
		this.user = user;
	}
	
	public static void main(String []args) {
		bak();
	}
	
	public static void bak() {
		try {
			
			XmlDocument doc = XmlHelper.loadXml("c:\\User\\test\\aaa.xml");
			XmlNode node = doc.selectSingleNode("//Student");
			Student student = XmlHelper.ToObject(node);
			System.out.println(student.getName());
		} catch (Exception e) {
			e.printStackTrace();
		}
	}
}
