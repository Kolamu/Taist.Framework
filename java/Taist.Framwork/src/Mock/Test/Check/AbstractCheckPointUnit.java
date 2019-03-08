package Mock.Test.Check;

import Mock.Data.TaistData;
import Mock.Data.Internal.TaistType;
import Mock.Data.Management.ExternalClassFactory;
import Mock.Data.Xml.XmlNode;
import Mock.Exception.CanNotFindTypeException;
import Mock.Test.TestContext;

public abstract class AbstractCheckPointUnit extends TaistData implements CheckPointUnit {
	protected XmlNode node = null;
	protected TestContext cache = null;
	private AssertOperator operator = null;
	private StringBuffer buf = null;
	
	public AbstractCheckPointUnit(XmlNode node, TestContext cache) {
		this.node = node;
		this.cache = cache;
	}
	
	public String getOperator() {
		if(operator == null) {
			return null;
		}
		return operator.getClass().getSimpleName().replace("Operator", "");
	}
	
	public void setOperator(String operatorName) {
		try {
			TaistType type = ExternalClassFactory.getInstance().getType(operatorName.replace("Operator", "") + "Operator");
			operator = (AssertOperator) type.createInstance();
		} catch (CanNotFindTypeException e) {
			e.printStackTrace();
			operator = null;
		}
	}
	
	@Override
	public CheckResult check() {
		buf = new StringBuffer();
		if(node == null) {
			return new DefaultCheckResult(null);
		}
		
		for(XmlNode node : node.getChildNodes()) {
			if(node.isComment()) {
				continue;
			}
			
			String executeValue = null;
			try {
				executeValue = cache.getContext(node.getName());
			} catch (Exception e) {
				e.printStackTrace();
			}
			String predictValue = node.getInnerXml();
			check(executeValue, predictValue);
		}
		return new DefaultCheckResult(buf.toString());
	}

	@Override
	public void writeBack() { }
	
	protected void check(Object obj1, Object obj2) {
		if(operator == null) {
			operator = new EqualsOperator();
		}
		if(!operator.run(obj1, obj2)) {
			buf.append(String.format("", obj1, obj2));
		}
	}
}
