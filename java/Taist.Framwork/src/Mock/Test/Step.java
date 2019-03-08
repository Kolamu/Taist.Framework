package Mock.Test;

import java.util.List;

import Mock.Config;
import Mock.Data.CanNotFindDataException;
import Mock.Data.TaistData;
import Mock.Data.Helpers.StringHelper;
import Mock.Data.Internal.CannotFindMethodException;
import Mock.Data.Internal.MethodInvokeException;
import Mock.Data.Management.CheckDataFactory;
import Mock.Data.Management.KeywordsFactory;
import Mock.Data.Management.TestCaseFactory;
import Mock.Exception.CanNotFindTypeException;
import Mock.Exception.InvalidDataTypeException;
import Mock.Exception.InvalidParamValueException;
import Mock.Test.Check.CheckPoint;

/**
 * 
 * 
 * @author Kolamu
 *
 */
public class Step extends TaistData {
	
	private String id = null;
	private String name = null;
	private String dataList = null;
	private String targetDataList = null;
	private String keywords = null;
	private String subKeywords = null;
	private String targetProject = null;

	public String getId() {
		return id;
	}
	public void setId(String id) {
		this.id = id;
	}
	public String getName() {
		return name;
	}
	public void setName(String name) {
		this.name = name;
	}
	public String getDataList() {
		return dataList;
	}
	public void setDataList(String dataList) {
		this.dataList = dataList;
	}
	public String getTargetDataList() {
		return targetDataList;
	}
	public void setTargetDataList(String targetDataList) {
		this.targetDataList = targetDataList;
	}
	public String getKeywords() {
		return keywords;
	}
	public void setKeywords(String keywords) {
		this.keywords = keywords;
	}
	public String getSubKeywords() {
		return subKeywords;
	}
	public void setSubKeywords(String subKeywords) {
		this.subKeywords = subKeywords;
	}
	public String getTargetProject() {
		return targetProject;
	}
	public void setTargetProject(String targetProject) {
		this.targetProject = targetProject;
	}
	
	public void run() {
		try {
			if(StringHelper.equals(keywords, "check", true)) {
				check();
			}
			else if(TestCaseFactory.getInstance().contains(keywords)) {
				runTestCase();
			}
			else {
				runKeywords();
			}
		} catch(Throwable e) {
			e.printStackTrace();
		}
	}
	
	private void check()
			throws
			InvalidParamValueException,
			CanNotFindDataException,
			InvalidDataTypeException,
			CanNotFindTypeException,
			MethodInvokeException,
			CannotFindMethodException {
		List<String> bhlist = StringHelper.parseBhList(getDataList());
        for (String checkBh : bhlist) {
            CheckPoint cp = CheckDataFactory.getInstance().getData(checkBh);
            if(targetDataList == null) {
            	cp.check();
            }
            else {
            	List<String> targetlist = StringHelper.parseBhList(targetDataList);
            	for(String target : targetlist) {
            		cp.setDataBh(target);
            		cp.check();
            	}
            }
        }
    }
	
	private void runKeywords() throws CanNotFindDataException, MethodInvokeException {
		KeywordsFactory.getInstance().Invoke(getKeywordsString());
	}
	
	private void runTestCase() throws InvalidParamValueException, CanNotFindDataException, InvalidDataTypeException, CanNotFindTypeException, MethodInvokeException, CannotFindMethodException {
		TestCase subCase = TestCaseFactory.getInstance().getData(keywords);
		subCase.run();
	}
	
	private String getKeywordsString() {
		String keywords = null;
		keywords = keywords + "," + subKeywords + ",";
		if(targetProject == null || targetProject.isEmpty()) {
			keywords = keywords + Config.getTargetProject();
		}
		else {
			keywords = keywords + targetProject;
		}
		return keywords;
	}
}
