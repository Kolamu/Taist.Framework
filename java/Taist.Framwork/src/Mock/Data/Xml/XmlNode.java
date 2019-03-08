package Mock.Data.Xml;

public interface XmlNode {
	XmlNode selectSingleNode(String xpath);
	XmlNodeList selectNodes(String xpath);
	XmlNodeList getChildNodes();
	
	String getInnerText();
	String setInnerText(String text);
	
	String getInnerXml();
	String setInnerXml(String xml);
	
	String getOuterXml();
	String setOuterXml();
	
	String getValue();
	String setValue(String value);
	
	String getName();
	
	boolean isComment();
	
	XmlDocument getOwnerDocument();
	XmlNode getParent();
	
	XmlAttributeList getAttributes();
	XmlNode appendChild(XmlNode child);
	XmlNode replaceChild(XmlNode newChild, XmlNode oldChild);
}
