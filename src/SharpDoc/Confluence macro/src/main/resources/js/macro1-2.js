// This javascript is accessible only by the parent window elements


// Get the 'refresh' button
var actualiseLink = document.getElementById('macro-browser-preview-link');


// Click the 'refresh' button (this will call the macro 'execute' function with the current state of the parameters)
function actualise()
{
	actualiseLink.click();
}

