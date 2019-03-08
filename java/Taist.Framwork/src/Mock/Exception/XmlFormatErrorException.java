package Mock.Exception;

public class XmlFormatErrorException extends TaistException {
	/**
	 * 
	 */
	private static final long serialVersionUID = -3046364207520609969L;

	public XmlFormatErrorException(String xmlString) {
		super("Xml format error : " + xmlString);
	}
}
