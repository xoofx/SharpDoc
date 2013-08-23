package docLink;

/**
 * Class that allow to easily manage the javascript code inserted in a HTML page 
 */
public class JavascriptStringBuilder {

	/**
	 * The javascript code managed by this class
	 */
	private StringBuilder txt;
	
	
	/**
	 * Initialise the stringBuilder with a HTML script Tag
	 */
	public JavascriptStringBuilder() {
		txt = new StringBuilder("<script type='text/javascript'>");
	}
	
	/**
	 * Add the given javascript file to the javascript code
	 * before opening a new javascript section
	 * @param fileUrl The url of the javascript file to include
	 */
	public JavascriptStringBuilder(String fileUrl) {
		txt = new StringBuilder();
		insertJavascriptFile(fileUrl);
		txt.append("<script type='text/javascript'>");
	}
	
	/**
	 * Add the given text in the current javascript section
	 * @param javascriptTxt
	 */
	public void append(String javascriptTxt) {
		txt.append(javascriptTxt);
	}
	
	/**
	 * Close the current javascript section and return the stringbuilder content as a string
	 */
	public String toString() {
		txt.append("</script>");
		return txt.toString();
	}
	
	/**
	 * Include the given javascript file
	 * @param fileUrl
	 */
	public void insertJavascriptFile(String fileUrl)
	{
		txt.append(String.format("<script type='text/javascript' src='%s'></script>", fileUrl));
	}

}
