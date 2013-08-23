package docLink;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.net.URL;
import java.util.Map;
import java.util.TreeSet;
import java.util.regex.Pattern;

import com.atlassian.confluence.content.render.xhtml.ConversionContext;
import com.atlassian.confluence.macro.Macro;
import com.atlassian.confluence.macro.MacroExecutionException;
import com.atlassian.confluence.setup.settings.SettingsManager;
import com.atlassian.gzipfilter.org.apache.commons.lang.StringEscapeUtils;

/**
 * Confluence Macro that allow to easily insert a link to documentation pages
 * 
 * As simple explanation of how Confluence macro works:
 * 
 * The user fills some parameters fields (an HTML form with mainly text fields and check-boxes)
 * Actualise the macro by clicking on the refresh button
 * The macro 'execute' function is called, it's parse the parameters to return HTML code that will be inserted in the confluence page 
 * 
 * For more explanations, please refer to the Atlasian documentation
 */
public class Macro1 implements Macro
{

	public Macro1()
	{
		
	}
	
	/**
	 * Allow to know some basic information about the macro Confluence environment 
	 */
	private SettingsManager mySettingsManager = null;
	
	/**
	 * The base url of the macro Confluence server (e.g. 'https://company.name.com/confluence/') 
	 */
	private String confluenceUrl;
	
	/**
	 * The url where macro resource are stored
	 */
	private String macroResourceUrl = "/download/resources/docLink.macro1";
	
	/**
	 * The last correct project url saved by the macro
	 */
	private String savedProjectPath = null;
	
	/**
	 * The current project url
	 */
	private String projectPath = null;
	
	/**
	 * The list of documentation file for the current project
	 */
	private TreeSet<ProjectFile> projectFiles = null;
		
	
	
	/**
	 * This method is called by Confluence at the macro creation
	 * @param settingsManager The Confluence settings 
	 */
	public void setSettingsManager(SettingsManager settingsManager)
	{
	    mySettingsManager = settingsManager;
	    confluenceUrl = mySettingsManager.getGlobalSettings().getBaseUrl();
	    macroResourceUrl = confluenceUrl + macroResourceUrl;
	}
	
	
	

	/**
	 * This function is called when the user click on 'refresh' (or by javascript command to automatically refresh the page)
	 * 
	 * According to parameter states, display instruction to the user to create the documentation link:
	 * 
	 * step 1 : choice of the Project
	 * 		Ask the user to enter the Url of his project documentation
	 * 		while the 'project' parameter is empty, this instructions are displayed
	 * 		if an url is given
	 * 			- if the url is correct (a index.txt file could be found at this url) goto step 2
	 * 			- if the url is incorrect (no index.txt file at this url) display error message
	 * 
	 * step 2 : choice of the documentation page
	 * 		Display the list of the all documentation pages
	 * 		Ask the user to choose one of them
	 * 		while the 'pageUrl' parameter is empty, this instructions are displayed
	 * 		to click on one the proposed page automatically fill the 'pageUrl' and 'txt' fields
	 * 		The user could specify some criterions for the search (step 2')
	 * 		if a page is given
	 * 			- if the page is correct (is on the project list) goto step 3
	 * 			- if the page is incorrect (isn't in the project list) display error message
	 * 
	 * step 2' : filtered search
	 * 		If some criterions are given (type, parent, namespace or name) regular expressions are used to filter the list
	 * 		If no page correspond to the criterions, an error message is displayed
	 * 
	 * step 3 : choice of the link text
	 * 		By default the link text is filled when a page is choosen, but an other txt could be given
	 * 		If the 'txt' field is empty, instrcutions are given
	 * 		the user could click on 'insert' to validate the macro result (a specific <a> tag is created with some data about the documentation page)
	 * 
	 * @param parameters The value of the macro parameters when the user click on refresh (empty text-fields and checked boxes are not present in the Map)
	 * @param body (Not used)
	 * @param context (Not used)
	 * @return The HTML code display by the macro
	 */
	@Override
	public String execute(Map<String, String> parameters, String body, ConversionContext context) throws MacroExecutionException
	{			
		StringBuilder txt = new StringBuilder(String.format("<link rel='stylesheet' type='text/css' href='%s'/>", macroResourceUrl + "/css/macro1.css"));
		JavascriptStringBuilder javascript = new JavascriptStringBuilder(macroResourceUrl + "/js/macro1.js");
		
		javascript.append(String.format("addParentJS('%s');", macroResourceUrl + "/js/macro1-2.js"));
		javascript.append("addInteractivity();");
		
		
		if(parameters.containsKey("project") || savedProjectPath!= null)
		{
			projectPath = parameters.containsKey("project") ? parameters.get("project") : savedProjectPath;
			
			if(!projectPath.endsWith("/"))
				projectPath = projectPath + "/";
			
			if(!projectPath.equals(savedProjectPath) || projectPath == null ||projectFiles == null)
			{				
				BufferedReader projectDoc;
			
				try
				{
					URL projectDocUrl = new URL(projectPath + "index.txt");
					projectDoc = new BufferedReader(new InputStreamReader(projectDocUrl.openStream()));
					if(!projectDoc.ready())
						throw new Exception();
				}
				catch(Exception e)
				{				
					txt.append("<div class='title'/> Error </div>");
					txt.append("<div class='error'> No project documentation could be found at the given Url .</div>");
					txt.append(String.format("<br/><div class='subtitle'/> Make sure that the index.txt file is present at %s. </div>", projectPath));
					
					javascript.append("setFocusOn('project');");
					return javascript.toString() + txt.toString();
				}
			
				try
				{
					projectFiles = new TreeSet<ProjectFile>(new ProjectFileComparator());

					String line = null;
					while((line = projectDoc.readLine()) != null)
					{
						ProjectFile projectFile = new ProjectFile(line);
						projectFiles.add(projectFile);
					}
				}
				catch (IOException e) {txt.append(String.format("reading error : %s ", e.getMessage()));}
				
				if(projectFiles != null && projectFiles.size() != 0)
					savedProjectPath = projectPath;
			}
			else
				javascript.append(String.format("setProjectUrl('%s');", savedProjectPath));
			
			if(!parameters.containsKey("pageUrl"))
			{			
				txt.append("<div class='title'/> Select a documentation file. </div>");
				txt.append("<div class='subtitle'/> You can filter your search by adding some criterions. </div>");
							
				if(!filterAreDefined(parameters))
				{
					txt.append("<p class='result'>The project contains the following documentation files: </p>");
					txt.append("<ul>");
					
					for(ProjectFile file : projectFiles)
					{
						txt.append(String.format("<li onclick='sendToPageUrl(event)' class='page' file='%s' name='%s'>%s</li>", file.url, file.name, file.formatedName));
					}
					txt.append("</ul>");
				}
				else
				{
					TreeSet<ProjectFile> filteredFiles = getFilteredFilesNames(projectFiles, parameters);
					
					if(filteredFiles.size() > 0)
					{
						txt.append("<p class='result'> The following files correspond to your criterions: </p>");
						txt.append("<ul>");
						for (ProjectFile file : filteredFiles)
						{
							txt.append(String.format("<li onclick='sendToPageUrl(event)' class='page' file='%s' name='%s'>%s</li>", file.url, file.name, file.formatedName));
						}
						txt.append("</ul>");
					}
					else
						txt.append("<p>No file corresponds to your criterions. </p>");
				}
			}
			else
			{
				String pageUrl = parameters.get("pageUrl");
				ProjectFile correspondingFile = getCorrespondingFile(projectFiles, pageUrl);
				if(correspondingFile != null)
				{
					if(parameters.containsKey("txt"))
					{
						String style = "background:	none repeat scroll 0% 0% rgb(245, 245, 245); border: 1px solid rgb(221, 221, 221); padding: 0px 2px; border-radius:	3px 3px 3px 3px; text-decoration: none;";
						
						return String.format("<a class=\"classLink\" page=\"%s\" name=\"%s\" href=\"%s/html/%s\" style='%s'>%s</a> ", pageUrl, StringEscapeUtils.escapeHtml(correspondingFile.name), projectPath, pageUrl, style, StringEscapeUtils.escapeHtml(parameters.get("txt")));
					}
					else
					{
						txt.append("<div class='title'/> Enter the text of the link. </div>");
						txt.append("<div class='subtitle'/> After this step you could insert the macro. </div>");
						
						javascript.append("setFocusOn('txt');");
					}
				}
				else
				{
					txt.append(String.format("<div class='title'/> Error </div> <div class='error'>'%s' is not a documentation file for the project.</div>", pageUrl));
					
					javascript.append("setFocusOn('pageUrl');");
				}
			}
			
			return javascript.toString() + txt.toString();
		}
		else
		{			
			txt.append("<div class='title'/> Enter the path of your project documentation. </div>");
			txt.append("<div class='subtitle'/> (Root url of your documentation, where the 'index.txt' file could be found).</div>");
			txt.append("<br/><br/><br/><br/><div class='title'/> Warning: </div> <div class='subtitle'> If you validate the macro before the creation of the link, the displayed text will be inserted ... </div>");
			txt.append("<div class='subtitle'/> The page will be automatically refreshed after each change.</div>");
			
			javascript.append("setFocusOn('project');");
			return javascript.toString() + txt.toString(); 
		}
	}

	/**
	 * As this macro doesn't need a body, return NONE
	 */
	@Override
	public BodyType getBodyType()
	{
		return BodyType.NONE;
	}
	
	/**
	 * As the macro is used to insert a link, the output type is INLINE
	 */
	@Override
	public OutputType getOutputType()
	{
		return OutputType.INLINE;
	}
	
	
	/**
	 * return true if one filtered search parameters has been filled
	 * @param parameters The macro parameters
	 * @return true if one filtered search parameters ('type', 'namespace', 'parent' or 'name' is not empty
	 */
	public boolean filterAreDefined(Map<String, String> parameters)
	{
		return parameters.containsKey("type") || parameters.containsKey("namespace") || parameters.containsKey("parent") || parameters.containsKey("name");
	}
	
	
	/**
	 * Return the ProjectFile of the given list that have the given url
	 * @param projectFiles A list of ProjectFile in which we search a particular url
	 * @param searchedUrl The url searched in the4 list
	 * @return the ProjectFile of the given list that have the given url
	 */
	public ProjectFile getCorrespondingFile(TreeSet<ProjectFile> projectFiles, String searchedUrl)
	{
		for(ProjectFile file : projectFiles)
		{
			if(file.url.equals(searchedUrl))
				return file;
		}
		return null;
	}
	
	/**
	 * Return a subSet of the given ProjectFile set that match the given parameters	
	 * @param projectFiles A set of ProjectFile
	 * @param parameters The filter parameters
	 * @return a subSet of the given ProjectFile set that match the given parameters	
	 */
	public TreeSet<ProjectFile> getFilteredFilesNames(TreeSet<ProjectFile> projectFiles, Map<String, String> parameters)
	{
		TreeSet<ProjectFile> filteredFilesName = new TreeSet<ProjectFile>( new ProjectFileComparator());
		
		String type = null;
		String namespace = null;
		String parent = null;
		String name = null;
		String pattern = null;
		
		if(parameters.containsKey("type"))
			type = parameters.get("type");
		
		/**
		 * The given criterions could be in the middle of the word
		 * so the given text is inserted in the regex pattern
		 */
		
		
		/**
		 * namespace names are only in alpha-numerics 
		 */
		if(parameters.containsKey("namespace"))
			namespace = "\\w*" + parameters.get("namespace") + "\\w*";
		else
			namespace = "\\w+";
		
		/**
		 * parent names are alpha-numerics with "<" "," " " ">"  for the generics
		 * if no parent name is given, the dot "." is facultative	 
		 * else if a parent name is given the dot "." is not facultative
		 */
		if(parameters.containsKey("parent"))
			parent = "[\\w<>, ]*" + parameters.get("parent") + "[\\w<>, ]*\\.";
		else
			parent = "[\\w<>, ]*\\.?";
		
		/**
		 * object names are alpha-numerics with "<" "," " " ">"  for the generics and "(" "," " " ")" for parameters
		 */
		if(parameters.containsKey("name"))
			name = "[\\w<>\\(\\), ]*" + parameters.get("name") + "[\\w<>\\(\\), ]*";
		else
			name = "[\\w<>\\(\\), ]+";
		
		
		
		
		if(type == null)
		{
			type = "\\w+";
			
			/**
			 * If the type is unknown, the general form is used 'parent'.'name' 'type' ('namespace')
			 */
			pattern = String.format("%s%s %s \\(%s\\)", parent, name, type, namespace);	
		}
		else
		{
			if(type.equals("Api"))
			{
				/**
				 * If the type is "Api", the pattern select the particular pages
				 */
				pattern = "Class Library Reference|index";
			}
			else if(type.equals("Class") || type.equals("Struct") || type.equals("Interface") || type.equals("Enumeration"))
			{
				/**
				 * If the type is a 'type' (Class, struct, interface or enumeration)
				 * the parent part is omitted
				 */
				pattern = String.format("%s %s \\(%s\\)", name, type, namespace);
			}
			else if(type.equals("Constructor"))
			{
				/**
				 * If the type is 'Constructor'
				 * the name of the object is the same than the parent (with some extra parameters)
				 */
				pattern = String.format("(?<parent>%s).\\k<parent>\\([\\w<>, ]*\\) %s \\(%s\\)", parent, "Method", namespace);
			}
			else if(type.equals("Delegate"))
			{
				/**
				 * If the type is 'Delegate'
				 * the name of the object could exceptionally contains a dot "."
				 */
				name = name.replace("[\\w<>\\(\\), ]", "[\\w<>\\(\\), \\.]");
				pattern = String.format("%s%s %s \\(%s\\)", parent, name, type, namespace);
			}
			else
			{
				/**
				 * If the type is a member (method, field, event, property), the general form is used 'parent'.'name' 'type' ('namespace')
				 */
				pattern = String.format("%s%s %s \\(%s\\)", parent, name, type, namespace);
			}
		}
		
		/**
		 * To make the search more simple, the case is ignored
		 */
		Pattern caseInsentivePattern = Pattern.compile(pattern, Pattern.CASE_INSENSITIVE);
		
		for (ProjectFile projectfile : projectFiles)
		{
			if(caseInsentivePattern.matcher((projectfile.name)).matches())
				filteredFilesName.add(projectfile);
		}
				
		return filteredFilesName;
	}
}
