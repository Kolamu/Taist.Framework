package Mock.Test.Check;

import Mock.Data.Helpers.StringHelper;

public final class DefaultCheckResult implements CheckResult {
	private String detail = null;
	public DefaultCheckResult(String detail) {
		this.detail = detail;
	}
	@Override
	public boolean isSuccess() {
		return StringHelper.isNullOrEmpty(detail);
	}

	@Override
	public String getDetail() {
		return detail;
	}

}
