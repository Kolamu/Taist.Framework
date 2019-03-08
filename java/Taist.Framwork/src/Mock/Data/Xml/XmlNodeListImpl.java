package Mock.Data.Xml;

import java.util.Iterator;

import org.w3c.dom.NodeList;

final class XmlNodeListImpl implements XmlNodeList {
	private NodeList xmlNodeList = null;
	public XmlNodeListImpl(NodeList list) {
		this.xmlNodeList = list;
	}
	
	@Override
	public XmlNode item(int index) {
		return new XmlNodeImpl(xmlNodeList.item(index));
	}
	
	@Override
	public int getLength() {
		return xmlNodeList.getLength();
	}

	@Override
	public Iterator<XmlNode> iterator() {
		// TODO Auto-generated method stub
		class XmlIterator implements Iterator<XmlNode>{
			private int curIndex = 0;
			@Override
			public boolean hasNext() {
				// TODO Auto-generated method stub
				return curIndex < xmlNodeList.getLength();
			}

			@Override
			public XmlNode next() {
				// TODO Auto-generated method stub
				return item(curIndex++);
			}
		}
		return new XmlIterator();
	}
}
