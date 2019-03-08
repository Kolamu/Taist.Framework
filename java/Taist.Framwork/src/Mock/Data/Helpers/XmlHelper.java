package Mock.Data.Helpers;

import Mock.Data.CanNotFindDataException;
import Mock.Data.Internal.CannotFindMethodException;
import Mock.Data.Internal.MethodInvokeException;
import Mock.Data.Internal.TaistMethod;
import Mock.Data.Internal.TaistProperty;
import Mock.Data.Internal.TaistType;
import Mock.Data.Management.CaseDataFactory;
import Mock.Data.Management.DataClassFactory;
import Mock.Data.Xml.XmlAttribute;
import Mock.Data.Xml.XmlDocument;
import Mock.Data.Xml.XmlNode;
import Mock.Exception.CanNotFindTypeException;
import Mock.Exception.InvalidDataTypeException;
import Mock.Exception.InvalidParamValueException;
import Mock.Exception.XmlFormatErrorException;

/**
 * 
 * This helper class provide some useful tools to make Xml data more easier. 
 * 
 * @author Kolamu
 * @version 1.0.20180616
 *
 */
public class XmlHelper {
	
	/**
	 * 
	 * Load xml file/content-string into XmlDocument.
	 * 
	 * This method will load xmlString as a file path first.
	 * If xmlString is not a legitimate file path, it will reload xmlString as a xml content.
	 * 
	 * @param xmlString
	 * xmlString can be an XML format file or a <type>String</type>
	 * which contains a complete XML content
	 * 
	 * @return return an XmlDocument instance
	 * @throws XmlFormatErrorException
	 * If xmlString is neither a legitimate file path, nor a right xml format String, this
	 * method will throw XmlFormatErrorException
	 */
	public static XmlDocument loadXml(String xmlString)
			throws
			XmlFormatErrorException {
		
		XmlDocument doc = new XmlDocument();
		try {
			if(FileHelper.Exists(xmlString)) {
				doc.load(xmlString);
			}
			else {
				doc.loadXml(xmlString);
			}
		}catch (Exception e) {
			throw new XmlFormatErrorException(xmlString);
		}
		return doc;
	}
	
	@SuppressWarnings("unchecked")
	public static <T> T ToObject(XmlNode typeNode)
			throws
			InvalidDataTypeException,
			InvalidParamValueException,
			CanNotFindTypeException,
			MethodInvokeException,
			CannotFindMethodException {
		return (T)ToObject(typeNode, null);
	}
	
	public static <T> T ToObject(XmlNode typeNode, T instance)
			throws
			InvalidDataTypeException,
			InvalidParamValueException,
			CanNotFindTypeException,
			MethodInvokeException,
			CannotFindMethodException {
		return (T)ToObject(typeNode, instance, null);
	}
	
	public static <T> T ToObject(XmlNode typeNode, T instance, Class<?> realType)
			throws
			InvalidDataTypeException,
			InvalidParamValueException,
			CanNotFindTypeException,
			MethodInvokeException,
			CannotFindMethodException {
		if(realType == null) {
			return (T)ToObject(typeNode, null, instance);
		}
		
		if(realType.isInterface()) {
			throw new InvalidDataTypeException("type can not be interface");
		}
		return (T)ToObject(typeNode, DataClassFactory.getInstance().getType(realType.getName()), instance);
	}
	
	public static <T> T ToObject(XmlNode typeNode, Class<?> type)
			throws
			InvalidDataTypeException,
			InvalidParamValueException,
			CanNotFindTypeException,
			MethodInvokeException,
			CannotFindMethodException {
		T instance = null;
		return (T)ToObject(typeNode, instance, type);
	}

	@SuppressWarnings("unchecked")
	private static <T> T ToObject(XmlNode typeNode, TaistType realType, T instance)
			throws
			InvalidDataTypeException,
			InvalidParamValueException,
			CanNotFindTypeException,
			MethodInvokeException,
			CannotFindMethodException {
		
		typeNode = InitXmlNode(typeNode);
		String typeName = typeNode.getName();
		
		TaistType type = GetRealType(typeName, instance, realType);
		
		T retObject = instance;
		
		if(retObject == null) {
			retObject = (T)type.createInstance();
		}
		
		try {
			initDefaultInstance(typeNode, retObject);
		} catch (CanNotFindDataException e) {
			e.printStackTrace();
		}
		
		for(XmlNode propertyNode : typeNode.getChildNodes()) {
			if(propertyNode.isComment()) {
				continue;
			}
			setNodeToObject(propertyNode, type, retObject);
		}
		
		for(XmlAttribute propertyAttribute : typeNode.getAttributes()) {
			setAttributeToObject(propertyAttribute, type, retObject);
		}
		
		return retObject;
	}
	
	public static String FromObject(Object obj) {
		return null;
	}
	
    private static TaistType GetRealType(String nodeName, Object instance, TaistType targetType) throws InvalidDataTypeException, CanNotFindTypeException
    {
    	if(nodeName == null) {
    		nodeName = "";
    	}
    	
        if (nodeName.isEmpty() && instance == null && targetType == null) {
            throw new InvalidDataTypeException("null");
        }

        TaistType realType = GetTypeFromInstance(instance, targetType);

        if (realType == null)
        {
            realType = DataClassFactory.getInstance().getType(nodeName);
        }

        if (nodeName.isEmpty())
        {
            return realType;
        }
        
        String realTypeName = realType.getName().toUpperCase();
        String realTypeFriendlyName = realType.getFriendlyName().toUpperCase();
        
        String typeName = nodeName.toUpperCase();

        if (realTypeFriendlyName.equals(typeName))
        {
            return realType;
        }

        if (realTypeName.equals(typeName))
        {
            return realType;
        }
        
        if (realTypeName.endsWith("IMPL") && realTypeName.substring(0, realTypeName.length()-4).equals(typeName))
        {
            return realType;
        }

        throw new InvalidDataTypeException(realTypeName);
    }
    
    private static void initDefaultInstance(XmlNode node, Object instance)
    		throws
    		InvalidDataTypeException,
    		InvalidParamValueException,
    		CanNotFindTypeException,
    		MethodInvokeException,
    		CannotFindMethodException,
    		CanNotFindDataException {
    	XmlNode defaultDataNode = node.selectSingleNode("DefaultDataBh");
    	if(defaultDataNode == null) {
    		return;
    	}
		defaultDataNode = CaseDataFactory.getInstance()
				.getData(defaultDataNode.getInnerText());
    	
    	if(defaultDataNode == null) {
    		return;
    	}
    	
    	ToObject(defaultDataNode, instance);
    }

    private static TaistType GetTypeFromInstance(Object objectInstance, TaistType targetType) throws CanNotFindTypeException
    {
        if (objectInstance == null)
        {
            return targetType;
        }
        else
        {
            return DataClassFactory.getInstance().getType(objectInstance.getClass().getName());
        }
    }

    private static XmlNode InitXmlNode(XmlNode objectNode) throws InvalidParamValueException
    {
        XmlNode initNode = null;
        if (objectNode == null)
        {
            throw new InvalidParamValueException("The objectNode is null");
        }

        if (objectNode.isComment())
        {
            throw new InvalidParamValueException("The objectNode is XmlComment");
        }

        initNode = objectNode;
        if (objectNode instanceof XmlDocument)
        {
            initNode = ((XmlDocument)objectNode).getDocumentElement();
        }
        return initNode;
    }

    private static void setNodeToObject(XmlNode propertyNode, TaistType type, Object instance) throws MethodInvokeException, CannotFindMethodException {
    	String propertyName = propertyNode.getName();
    	TaistProperty property = type.getProperty(propertyName);
    	LogHelper.Debug("set property " + propertyName + " = " + propertyNode.getInnerText());
    	if(property == null) {
    		TaistMethod method = type.getMethod("putDynamicProperty");
    		if(method == null) {
    			LogHelper.Debug("Property named %s ignored because can not find property from Type named %s", propertyName, type.getName());
    		}
    		else {
    			method.Invoke(instance, propertyNode);
    		}
    	}
    	else {
    		property.setValue(instance, propertyNode.getInnerText());
    	}
    }
    
    private static void setAttributeToObject(XmlAttribute propertyAttribute, TaistType type, Object instance) throws CannotFindMethodException, MethodInvokeException {
    	String propertyName = propertyAttribute.getName();
    	TaistProperty property = type.getProperty(propertyName);
    	System.out.println("set property " + propertyName + " = " + propertyAttribute.getValue());
    	if(property == null) {
    		System.out.println("Property" + propertyName + "not found");
    	}
    	else {
    		property.setValue(instance, propertyAttribute.getValue());
    	}
    }
    
//    private static XmlNode NodeStandalone(XmlNode xmlNode)
//    {
//        XmlNode retNode = xmlNode.OwnerDocument.CreateElement("Data");
//        retNode.AppendChild(xmlNode.Clone());
//        return retNode;
//    }

//    private static TaistType GetDefaultData(XmlNode objectXmlNode, TaistType dataType)
//    {
//        XmlNode defaultDataBhNode = objectXmlNode.SelectSingleNode("DefaultDataBh");
//
//        if (defaultDataBhNode == null)
//        {
//            return dataType;
//        }
//
//        if (string.IsNullOrEmpty(defaultDataBhNode.InnerText))
//        {
//            return dataType;
//        }
//
//        try
//        {
//            XmlNode defaultDataNode = TestCasePool.GetDataNode(defaultDataBhNode.InnerText.Trim());
//            object defaultData = GetData(defaultDataNode, dataType);
//            return DataType.GetDataType(defaultData);
//        }
//        catch (CanNotFindDataException)
//        {
//            return dataType;
//        }
//    }
}
