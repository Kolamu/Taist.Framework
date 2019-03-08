package Mock.Data.Xml;

import org.w3c.dom.Node;

final class XmlAttributeImpl implements XmlAttribute {
	private Node attributeNode = null;
	
	public XmlAttributeImpl(Node attributeNode) {
		// TODO Auto-generated constructor stub
		this.attributeNode = attributeNode;
	}
	
	@Override
	public String getValue() {
		// TODO Auto-generated method stub
		return attributeNode.getNodeValue();
	}

	@Override
	public void setValue(String value) {
		// TODO Auto-generated method stub
		attributeNode.setNodeValue(value);
	}

	@Override
	public String getName() {
		// TODO Auto-generated method stub
		return attributeNode.getNodeName();
	}
}
