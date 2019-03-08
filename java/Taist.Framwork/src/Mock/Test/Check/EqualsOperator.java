package Mock.Test.Check;

public class EqualsOperator implements AssertOperator {

	public boolean run(Object op1, Object op2) {
		if(op1 == null && op2 == null) {
			return true;
		}
		
		if(op1 != null) {
			return op1.equals(op2);
		}
		
		if(op2 != null) {
			return op2.equals(op1);
		}
		return false;
	}
}
