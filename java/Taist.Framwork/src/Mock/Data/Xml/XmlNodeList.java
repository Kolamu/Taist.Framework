package Mock.Data.Xml;

public interface XmlNodeList extends Iterable<XmlNode> {
	XmlNode item(int index);
	int getLength();
}
