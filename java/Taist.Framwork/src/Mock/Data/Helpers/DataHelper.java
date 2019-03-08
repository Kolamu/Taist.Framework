package Mock.Data.Helpers;

import java.util.Map;

import Mock.Data.CanNotFindDataException;
import Mock.Data.TaistData;
import Mock.Data.Internal.CannotFindMethodException;
import Mock.Data.Internal.MethodInvokeException;
import Mock.Data.Xml.XmlNode;
import Mock.Exception.CanNotFindTypeException;
import Mock.Exception.InvalidDataTypeException;
import Mock.Exception.InvalidParamValueException;

public class DataHelper {
	public static <E> E GetData(XmlNode node)
			throws
			InvalidParamValueException,
			CanNotFindDataException,
			InvalidDataTypeException,
			CanNotFindTypeException,
			MethodInvokeException,
			CannotFindMethodException {
		if(node == null) {
			return null;
		}
		E e = XmlHelper.ToObject(node);
		if(e instanceof TaistData) {
			((TaistData)e).init();
		}
		return e;
	}
	
	
	public static String GetXmlConditionString(String typeName, Map<String, String> conditions)
    {
        String conditionString = "";
        if (conditions == null || conditions.isEmpty()) {
            //return null;
            conditionString = String.format("//%s", typeName);
        }
        else {
        	
        	for(Map.Entry<String, String> kv : conditions.entrySet()){
        		if (conditionString == "")
                {
                    conditionString = String.format("//%s[%s='%s']", typeName, kv.getKey(), kv.getValue());
                }
                else
                {
                    conditionString = String.format("%s and %s='%s']", conditionString.substring(0, conditionString.length() - 1), kv.getKey(), kv.getValue());
                }
        	}
        }
        return conditionString;
    }
	
}
