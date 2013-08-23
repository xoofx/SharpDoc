package docLink;


import java.util.regex.Matcher;
import java.util.regex.Pattern;

import org.apache.commons.lang.StringEscapeUtils;

/**
 * Class that manage the data of a documentation object
 */
public class ProjectFile {

	/**
	 * The url of the file
	 */
	public String url;
	
	/**
	 * The name (definition) of the object described in the file
	 */
	public String name;
	
	/**
	 * A formated version of the 'name' field (the part of the name are wrapped in HTML tags)
	 */
	public String formatedName;
	
	/**
	 * The pattern used to parse the object definition
	 * For the real assembly objects (not all api or research pages)
	 * the definition follow the pattern:
	 * - the name  = the parent if exists + "." + object name ("\w" for the alpha-numeric part, "<" "," " " ">" for generics, "(" "," " " ")" for arguments)
	 * - the type  = Namespace, Class, Struct, Field, Method, ... ("\w" only alpha-numerics)
	 * - the namespace = ("(\w)" only alpha-numerics surrounded by parenthesis)
	 */
	private static final Pattern p = Pattern.compile("([\\w<>\\(\\), \\.]+) ([\\w]+) (\\(\\w+\\))");
	
	
	/**
	 * Parse the given String to set the project file fields
	 * 
	 * The string have to be formed as follow:
	 * - the url of the page
	 * - the definition of the object
	 * - an optional signature for functions
	 * 
	 * Each part is separated from the others by a pipe "|"
	 * 
	 * @param line The parsed file
	 */
	public ProjectFile(String line)
	{
		String fileInfo[] = line.split("\\|");
		if(fileInfo.length == 2 || fileInfo.length == 3)
		{
			url = fileInfo[0];
			
			name = fileInfo[1];
			if(fileInfo.length == 3)
			{
				String signature = fileInfo[2];
				int parenthesisIndex = signature.indexOf("(");
				String methodName = signature.substring(0, parenthesisIndex);
				name = name.replace(methodName + " ", signature + " ");
			}
			
			Matcher m = p.matcher(name);
			if(m.matches())
				formatedName = String.format("<span class='object'>%s</span> <span class='type'>%s</span> <span class='namespace'>%s</span>",  StringEscapeUtils.escapeHtml(m.group(1)), m.group(2), m.group(3));
			else
				formatedName = String.format("<span class='object'>%s</span>",  StringEscapeUtils.escapeHtml(name));
		}
		else
		{
			url="";
			name="";
			formatedName= String.format("<span class='object incorrect'>'%s'</span> <span class='type'>is a not correct documetation data (object Url|object name[|optional method signature])</span>", line);
		}
	}
}
