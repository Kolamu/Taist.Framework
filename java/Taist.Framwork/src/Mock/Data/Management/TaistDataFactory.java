package Mock.Data.Management;

import java.util.HashMap;
import java.util.List;
import java.util.Map;

import Mock.Data.CanNotFindDataException;
import Mock.Data.Helpers.DataHelper;
import Mock.Data.Helpers.FileHelper;
import Mock.Data.Helpers.StringHelper;
import Mock.Data.Helpers.XmlHelper;
import Mock.Data.Internal.CannotFindMethodException;
import Mock.Data.Internal.MethodInvokeException;
import Mock.Data.Xml.XmlDocument;
import Mock.Data.Xml.XmlNode;
import Mock.Data.Xml.XmlNodeList;
import Mock.Exception.CanNotFindTypeException;
import Mock.Exception.InvalidDataTypeException;
import Mock.Exception.InvalidParamValueException;
import Mock.Exception.XmlFormatErrorException;

public abstract class TaistDataFactory {
	private String dataDirectoryPath = null;
	private Map<String, XmlNode> dataCache = null;
	
	public TaistDataFactory(String dataDirectoryPath) {
		this.dataDirectoryPath = dataDirectoryPath;
		dataCache = new HashMap<String, XmlNode>();
		cacheData();
	}
	
	public boolean contains(String dataBh) {
		return dataCache.containsKey(dataBh);
	}
	
	public XmlNode getNode(String dataBh) throws InvalidParamValueException, CanNotFindDataException {
		if(StringHelper.isNullOrEmpty(dataBh)) {
			throw new InvalidParamValueException("dataBh is null");
		}
		
		if(dataCache.containsKey(dataBh)) {
			return dataCache.get(dataBh);
		}

		throw new CanNotFindDataException(dataBh);
	}
	
	public <E> E getData(String dataBh)
			throws
			InvalidParamValueException,
			CanNotFindDataException,
			InvalidDataTypeException,
			CanNotFindTypeException,
			MethodInvokeException,
			CannotFindMethodException {
		return DataHelper.GetData(getNode(dataBh));
	}
	
	private void cacheData() {
		List<String> dataFileNameList = FileHelper.GetAllFileNames(dataDirectoryPath, ".xml");
		
		for(String fileName : dataFileNameList) {
			try {
				XmlDocument dataDocument = XmlHelper.loadXml(fileName);
				XmlNodeList bhNodeList = dataDocument.selectNodes("//Bh");
				for (XmlNode bhNode : bhNodeList) {
					dataCache.put(bhNode.getInnerText(), bhNode.getParent());
				}
			} catch (XmlFormatErrorException e) {
				e.printStackTrace();
			}
		}
	}
}
