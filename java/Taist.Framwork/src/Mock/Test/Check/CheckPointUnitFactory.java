package Mock.Test.Check;

import Mock.Data.Internal.TaistType;
import Mock.Data.Management.ExternalClassFactory;
import Mock.Data.Xml.XmlNode;
import Mock.Exception.CanNotFindTypeException;
import Mock.Test.TestContext;

class CheckPointUnitFactory {
	public static CheckPointUnit getCheckPointUnit(XmlNode node, TestContext cache) {
		try {
			TaistType type = ExternalClassFactory.getInstance().getType(node.getName());
			Object obj = type.createInstance(node, cache);
			if(obj instanceof CheckPointUnit) {
				return (CheckPointUnit)obj;
			}
		} catch (CanNotFindTypeException e) {
			e.printStackTrace();
		}
		return new DefaultCheckPointUnit(node, cache);
	}
}
