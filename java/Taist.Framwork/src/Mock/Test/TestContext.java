package Mock.Test;

import java.util.Map;

import Mock.Data.CanNotFindDataException;
import Mock.Data.TaistData;
import Mock.Data.Helpers.FileHelper;
import Mock.Data.Helpers.XmlHelper;
import Mock.Data.Xml.XmlDocument;
import Mock.Data.Xml.XmlNode;
import Mock.Exception.XmlFormatErrorException;
/**
 * TestContext 主要负责记录测试过程中产生的临时对象
 * 
 * @author Kolamu
 *
 */
public class TestContext extends TaistData {
	private String path = null;
	public TestContext() {
	}
	
	public TestContext(String bh) {
		setBh(bh);
	}
	
	public void save() {
		XmlDocument doc = new XmlDocument();
        if (!FileHelper.Exists(path)) {
        	FileHelper.createFile(path);
            doc.loadXml("<?xml version=\"1.0\" encoding=\"gbk\"?><Data></Data>");
        }
        else
        {
            doc.load(path);
        }
        
        XmlDocument cache;
		try {
			cache = XmlHelper.loadXml(this.toXml());
		} catch (XmlFormatErrorException e) {
			e.printStackTrace();
			return;
		}
        
        XmlNode cacheNode = doc.importNode(cache.getDocumentElement(), true);
        XmlNode xn = doc.selectSingleNode(String.format("//TestContext[Bh='%s']", getBh()));
        if (xn == null) {
        	doc.getDocumentElement().appendChild(cacheNode);
        }
        else {
            for (XmlNode tmp : cache.getDocumentElement().getChildNodes()) {
                XmlNode cacheChild = cacheNode.selectSingleNode(tmp.getName());
                XmlNode child = xn.selectSingleNode(cacheChild.getName());
                if (child == null) {
                    xn.appendChild(cacheChild);
                }
                else {
                    xn.replaceChild(cacheChild, child);
                }
            }
        }
        doc.save(path);
	}
	
	public void refresh() {
		try {
			if (!FileHelper.Exists(path)) {
                return;
            }

            XmlDocument doc = XmlHelper.loadXml(path);

            XmlNode xn = doc.selectSingleNode(String.format("//Context[Bh='%s']", getBh()));
            if (xn == null) {
                return;
            }

            XmlHelper.ToObject(xn, this);
		} catch (Exception e) {
			e.printStackTrace();
		}
	}
	
	public String getContext(String name)
			throws
			XmlFormatErrorException,
			CanNotFindDataException {
		Map<String, XmlNode> data = getDynamicProperties();
		if (data == null) {
            throw new CanNotFindDataException(name);
        }
        if (data.containsKey(name)) {
            return data.get(name).getInnerXml();
        }
        
        if (name.contains(".")) {
            String infoName = name.substring(0, name.indexOf('.'));
            if (data.containsKey(infoName)) {
            	String infoValue = data.get(infoName).getInnerXml();
                String infoString = String.format("<%s>%s</%s>", infoName, infoValue, infoName);

                XmlDocument infoDoc = XmlHelper.loadXml(infoString);
                XmlNode node = infoDoc.selectSingleNode("//" + name.replace('.', '/'));
                if (node == null) {
                    throw new CanNotFindDataException(name);
                }
                return node.getInnerXml();
            }
        }
        throw new CanNotFindDataException(name);
	}

	public void setContext(String key, String value) {
		String xmlString = String.format("<%s>%s</%s>", key, value, key);
		
		try {
			XmlDocument doc = XmlHelper.loadXml(xmlString);
			putDynamicProperty(doc.getDocumentElement());
		} catch (XmlFormatErrorException e) {
			e.printStackTrace();
		}
	}
}
