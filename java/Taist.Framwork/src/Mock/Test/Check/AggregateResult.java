package Mock.Test.Check;

import java.util.LinkedList;
import java.util.List;

final class AggregateResult implements CheckResult {
	private List<CheckResult> resultList = new LinkedList<>();
	
	@Override
	public boolean isSuccess() {
		for(CheckResult result : resultList) {
			if(result.isSuccess()) {
				continue;
			}
			return false;
		}
		return true;
	}

	@Override
	public String getDetail() {
		StringBuffer sb = new StringBuffer();
		for(CheckResult result : resultList) {
			if(!result.isSuccess()) {
				sb.append(result.getDetail());
				sb.append("\r\n");
			}
		}
		return sb.toString();
	}
	
	public void add(CheckResult result) {
		resultList.add(result);
	}
}
