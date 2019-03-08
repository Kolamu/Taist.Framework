package Mock.Data.Xml;

import java.util.Iterator;

import org.w3c.dom.NamedNodeMap;
import org.w3c.dom.Node;

final class XmlAttributeListImpl implements XmlAttributeList {
	private NamedNodeMap xmlAttributeList = null;
	
	public XmlAttributeListImpl(NamedNodeMap map) {
		xmlAttributeList = map;
	}
	
	@Override
	public Iterator<XmlAttribute> iterator() {
		class XmlAttributeIterator implements Iterator<XmlAttribute>{
			private int curIndex = 0;
			@Override
			public boolean hasNext() {
				return curIndex < xmlAttributeList.getLength();
			}

			@Override
			public XmlAttribute next() {
				// TODO Auto-generated method stub
				return item(curIndex++);
			}
			
		}
		
		return new XmlAttributeIterator();
	}

	@Override
	public XmlAttribute item(int index) {
		return new XmlAttributeImpl(xmlAttributeList.item(index));
	}

	@Override
	public XmlAttribute item(String name) {
		Node node = xmlAttributeList.getNamedItem(name);
		if(node == null) {
			return null;
		}
		return new XmlAttributeImpl(node);
	}

	@Override
	public int getLength() {
		return xmlAttributeList.getLength();
	}
}
