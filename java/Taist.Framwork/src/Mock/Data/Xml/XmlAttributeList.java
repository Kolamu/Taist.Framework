package Mock.Data.Xml;

public interface XmlAttributeList extends Iterable<XmlAttribute> {
	XmlAttribute item(int index);
	XmlAttribute item(String name);
	
	int getLength();
}
