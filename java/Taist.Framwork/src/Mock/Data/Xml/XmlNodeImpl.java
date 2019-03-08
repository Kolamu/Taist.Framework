package Mock.Data.Xml;

import java.io.StringWriter;

import javax.xml.transform.OutputKeys;
import javax.xml.transform.Transformer;
import javax.xml.transform.TransformerException;
import javax.xml.transform.TransformerFactory;
import javax.xml.transform.dom.DOMSource;
import javax.xml.transform.stream.StreamResult;
import javax.xml.xpath.XPathConstants;
import javax.xml.xpath.XPathExpressionException;
import javax.xml.xpath.XPathFactory;

import org.w3c.dom.Comment;
import org.w3c.dom.Node;
import org.w3c.dom.NodeList;

import Mock.Data.Helpers.StringHelper;

class XmlNodeImpl implements XmlNode{
	protected Node xmlNode = null;
	public XmlNodeImpl() {
	}
	
	public XmlNodeImpl(Node node) {
		this.xmlNode = node;
	}
	
	@Override
	public XmlNode selectSingleNode(String xpath) {
		try {
			return new XmlNodeImpl((Node)XPathFactory.newInstance().newXPath().evaluate(xpath, xmlNode, XPathConstants.NODE));
		} catch (XPathExpressionException e) {
			return null;
		}
	}

	@Override
	public XmlNodeList selectNodes(String xpath) {
		try {
			return new XmlNodeListImpl((NodeList)XPathFactory.newInstance().newXPath().evaluate(xpath, xmlNode, XPathConstants.NODESET));
		} catch (XPathExpressionException e) {
			return null;
		}
	}
	
	@Override
	public XmlNodeList getChildNodes() {
		return new XmlNodeListImpl(xmlNode.getChildNodes());
	}
	
	@Override
	public String getInnerText() {
		return StringHelper.replace("<.*?>", getInnerXml(), " ");
	}

	@Override
	public String setInnerText(String text) {
		return null;
	}

	@Override
	public String getInnerXml() {
		String outerXml = getOuterXml();
		String nodeName = xmlNode.getNodeName();
		
		return outerXml.substring(nodeName.length() + 2, outerXml.length() - nodeName.length() - 3);
	}

	@Override
	public String setInnerXml(String xml) {
		return null;
	}

	@Override
	public String getValue() {
		return null;
	}

	@Override
	public String setValue(String value) {
		return null;
	}

	@Override
	public String getOuterXml() {
		StringWriter writer = new StringWriter();
		try {
			Transformer trans = TransformerFactory.newInstance().newTransformer();
			trans.setOutputProperty(OutputKeys.OMIT_XML_DECLARATION, "yes");
			trans.transform(new DOMSource(xmlNode), new StreamResult(writer));
		}catch(TransformerException e) {
			e.printStackTrace();
		}
		
		return writer.toString();
	}

	@Override
	public String setOuterXml() {
		return null;
	}
	
	@Override
	public String getName() {
		return xmlNode.getNodeName();
	}
	
	@Override
	public boolean isComment() {
		return xmlNode instanceof Comment;
	}
	
	@Override
	public XmlDocument getOwnerDocument() {
		XmlDocument doc = new XmlDocument();
		doc.setDocument(xmlNode.getOwnerDocument());
		return doc;
	}

	@Override
	public XmlAttributeList getAttributes() {
		return new XmlAttributeListImpl(xmlNode.getAttributes());
	}

	@Override
	public XmlNode getParent() {
		return new XmlNodeImpl(xmlNode.getParentNode());
	}

	@Override
	public XmlNode appendChild(XmlNode child) {
		xmlNode.appendChild(((XmlNodeImpl)child).xmlNode);
		return child;
	}

	@Override
	public XmlNode replaceChild(XmlNode newChild, XmlNode oldChild) {
		xmlNode.replaceChild(((XmlNodeImpl)newChild).xmlNode, ((XmlNodeImpl)oldChild).xmlNode);
		return oldChild;
	}
	
}
