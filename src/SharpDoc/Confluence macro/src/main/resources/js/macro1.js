
// The macro is embedded in an iFrame, use this method to access to the main window DOM elements 
function $(id)
{
	return top.document.getElementById(id);
}


// Copy the 'file' value of the page link in the 'pageUrl' field
// Copy the 'name' value of the page link in the 'txt' field
function sendToPageUrl(event)
{
	var pageUrlField = $('macro-param-pageUrl');
	pageUrlField.value = event.currentTarget.getAttribute("file");
	var txtField = $('macro-param-txt');
	txtField.value = event.currentTarget.getAttribute("name");
	actualise();
}


// Set the given value to the 'project' field
function setProjectUrl(newUrl)
{
	var projectField = $("macro-param-project");
	projectField.value = newUrl;
}



// Make the focus on the given parameter
function setFocusOn(paramName)
{
	var paramField = $('macro-param-' + paramName);
	paramField.focus();
}


// Actualise the macro by clicking on the 'refresh' button (this will call the macro 'execute' function with the current state of the parameters)
function actualise()
{
	var actualiseLink = $('macro-browser-preview-link');
	actualiseLink.click();
}


// Add the given javascript file in the parent page 
function addParentJS(jsUrl)
{
	var parentDoc = top.document;
	var newJS = parentDoc.createElement("script");
	newJS.setAttribute("type", "text/javascript");
	newJS.setAttribute("src", jsUrl);
	parentDoc.head.appendChild(newJS);
}


// Add event handlers to the parameters (launch the 'actualise' function when a parameter change)
// The 'actualise' function is in the parent window context (see macro-1-2.js)
function addInteractivity()
{
	var paramField = $('macro-param-project');
	
	if(paramField.getAttribute("onkeyup") == null)
	{
		paramField.setAttribute("onkeyup", "actualise(event)");
		
		paramField = $('macro-param-pageUrl');
		paramField.setAttribute("onkeyup", "actualise(event)");
		
		paramField = $('macro-param-txt');
		paramField.setAttribute("onkeyup", "actualise(event)");
		
		paramField = $('macro-param-type');
		paramField.setAttribute("onchange", "actualise(event)");
		
		paramField = $('macro-param-namespace');
		paramField.setAttribute("onkeyup", "actualise(event)");
		
		paramField = $('macro-param-name');
		paramField.setAttribute("onkeyup", "actualise(event)");
		
		paramField = $('macro-param-parent');
		paramField.setAttribute("onkeyup", "actualise(event)");
	}
}