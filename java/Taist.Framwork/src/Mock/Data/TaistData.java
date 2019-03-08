package Mock.Data;

import java.util.HashMap;
import java.util.Map;

import Mock.Data.Helpers.DataHelper;
import Mock.Data.Helpers.XmlHelper;
import Mock.Data.Internal.CannotFindMethodException;
import Mock.Data.Internal.MethodInvokeException;
import Mock.Data.Xml.XmlNode;
import Mock.Exception.CanNotFindTypeException;
import Mock.Exception.InvalidDataTypeException;
import Mock.Exception.InvalidParamValueException;

public abstract class TaistData {
	private String bh = null;
	protected XmlNode dataNode = null;
	protected String dataFilePath = null;
	private Map<String, XmlNode> dynamicProperty = null;
	public String getBh() {
		return bh;
	}
	
	public void setBh(String value) {
		this.bh = value;
	}
	
	public void putDynamicProperty(XmlNode node) {
		if(dynamicProperty == null) {
			dynamicProperty = new HashMap<String, XmlNode>();
		}
		
		String propertyName = acquireDynamicPropertyName(node.getName());
		dynamicProperty.put(propertyName, node);
	}
	
	public Map<String, XmlNode> getDynamicProperties(){
		return dynamicProperty;
	}
	
	public XmlNode getDynamicProperty(String propertyName) throws CanNotFindDataException {
		if(dynamicProperty == null) {
			throw new CanNotFindDataException(propertyName);
		}
		if(dynamicProperty.containsKey(propertyName)) {
			return dynamicProperty.get(propertyName);
		}
		throw new CanNotFindDataException(propertyName);
	}
	
	private String acquireDynamicPropertyName(String name) {
		if(dynamicProperty == null) {
			return name;
		}
		
		int index = 0;
		String propertyName = name;
		while(dynamicProperty.containsKey(propertyName)) {
			index++;
			propertyName = name + index;
		}
		
		return propertyName;
	}
	
	public TaistData fromXml(XmlNode node, Map<String, String> conditions)
			throws
			InvalidDataTypeException,
			InvalidParamValueException,
			CanNotFindTypeException,
			MethodInvokeException,
			CannotFindMethodException,
			CanNotFindDataException {
		
		String typeName = this.getClass().getSimpleName();
		String conditionString = DataHelper.GetXmlConditionString(typeName, conditions);
		dataFilePath = node.getOwnerDocument().getFilePath();
		dataNode = node.selectSingleNode(conditionString);
		if(dataNode == null) {
			throw new CanNotFindDataException(conditionString);
		}
		return (TaistData)XmlHelper.ToObject(dataNode, this);
	}
	
	public String toXml() {
		return XmlHelper.FromObject(this);
	}
	
	public void init() {
		
	}
}
