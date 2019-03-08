package Mock.Data.Xml;

import java.io.ByteArrayInputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStreamWriter;
import java.io.UnsupportedEncodingException;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;
import javax.xml.transform.Result;
import javax.xml.transform.Source;
import javax.xml.transform.Transformer;
import javax.xml.transform.TransformerFactory;
import javax.xml.transform.dom.DOMSource;
import javax.xml.transform.stream.StreamResult;

import org.w3c.dom.Document;
import org.xml.sax.SAXException;

import Mock.Data.Helpers.FileHelper;
import Mock.Data.Helpers.StringHelper;

public final class XmlDocument extends XmlNodeImpl implements XmlNode{
	private Document document = null;
	private XmlNode documentElement = null;
	private String xmlFilePath = null;
	
	void setDocument(Document document) {
		this.document = document;
	}
	
	public void load(String xmlFileName) {
		try {
			File file = new File(xmlFileName);
			this.xmlFilePath = file.getAbsolutePath();
			byte[] content = new byte[(int)file.length()];
			FileInputStream input = new FileInputStream(file);
			input.read(content);
			input.close();
			
			loadXml(new String(content, FileHelper.getTXTFileEncoding(xmlFileName)));
		} catch (FileNotFoundException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}
	
	public void loadXml(String xmlContent) {
		try {
			xmlContent = formatContent(xmlContent);
			LoadFromStream(new ByteArrayInputStream(xmlContent.getBytes(getEncoding(xmlContent))));
		} catch (UnsupportedEncodingException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (ParserConfigurationException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (SAXException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}
	
	public void save(String xmlFileName){
		try {
			Source xmlSource = new DOMSource(document);
			TransformerFactory factory = TransformerFactory.newInstance();  
			Transformer transformer = factory.newTransformer();
			OutputStreamWriter writer = new OutputStreamWriter(new FileOutputStream(new File(xmlFileName)), document.getXmlEncoding());
			Result result = new StreamResult(writer);
			transformer.transform(xmlSource, result);
		} catch (Exception e) {
			e.printStackTrace();
		}
	}
	
	public XmlNode getDocumentElement() {
		return documentElement;
	}
	
	public String getFilePath() {
		return xmlFilePath;
	}
	
	public XmlNode importNode(XmlNode node, boolean inherit) {
		return new XmlNodeImpl(
				document.importNode(((XmlNodeImpl)node).xmlNode, inherit)
				);
	}
	
	private void LoadFromStream(InputStream is) throws ParserConfigurationException, SAXException, IOException {
		try { 
			DocumentBuilderFactory factory = DocumentBuilderFactory.newInstance();
			DocumentBuilder builder = factory.newDocumentBuilder();
			document = builder.parse(is);
			xmlNode = document.getDocumentElement();
			documentElement = new XmlNodeImpl(xmlNode);
		} finally {
			is.close();
		}
	}
	
	private String getEncoding(String xmlString) {
		Pattern pattern = Pattern.compile("<\\?.*?encoding=\"(.*?)\".*?>");
		Matcher matcher = pattern.matcher(xmlString);
		if(matcher.matches()) {
			return matcher.group(1);
		}
		else {
			return "UTF-8";
		}
	}
	
	private String formatContent(String content) {
		if(StringHelper.isNullOrEmpty(content)) {
			return content;
		}
		
		String result = StringHelper.replace(">\\s*?<", content, "><");
		return result;
	}
}
