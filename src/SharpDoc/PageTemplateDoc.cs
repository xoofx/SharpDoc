// Copyright (c) 2010-2013 SharpDoc - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using SharpDoc;
using SharpDoc.Logging;
using SharpDoc.Model;
using SharpRazor;

namespace SharpDoc
{
    /// <summary>
    /// Overrides default RazorEngine TemplateBase to provide additional
    /// methods (Import) and properties (Helpers, Param, Style).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageTemplateDoc : PageTemplate<TemplateContext>
    {
        private IDictionary<string, object> _helpersDictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="PageTemplateDoc"/> class.
        /// </summary>
        public PageTemplateDoc()
        {
            Helpers = new ExpandoObject();
            _helpersDictionary = (IDictionary<string, object>)Helpers;
        }

        /// <summary>
        /// Gets or sets the helpers.
        /// </summary>
        /// <value>The helpers.</value>
        public dynamic Helpers { get; private set; }

        /// <summary>
        /// Gets the param dynamic properties.
        /// </summary>
        /// <value>The param dynamic properties.</value>
        public dynamic Param { get { return Model.Param; } }

        /// <summary>
        /// Gets the style dynamic properties.
        /// </summary>
        /// <value>The style dynamic properties.</value>
        public dynamic Style { get { return Model.Style; }}

        /// <summary>
        /// Registers the helper.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="template">The template.</param>
        /// <param name="helperMethod">The helper method.</param>
        private void RegisterHelper(string name, PageTemplate template, MethodInfo helperMethod)
        {
            DynamicHelper dynamicHelper = null;

            if (!_helpersDictionary.ContainsKey(name))
            {
                dynamicHelper = new DynamicHelper();
                _helpersDictionary.Add(name, dynamicHelper);
            } else
            {
                dynamicHelper = (DynamicHelper)_helpersDictionary[name];
            }

            dynamicHelper.RegisterHelper(template, helperMethod);
        }

        /// <summary>
        /// Imports the template with the specified name.
        /// </summary>
        /// <param name="name">The template name.</param>
        /// <returns>
        /// The result of the template with the specified name.
        /// </returns>
        public virtual string Import(string name)
        {
            // If helpers is already loaded, then return immediately
            if (_helpersDictionary.ContainsKey(name))
                return "";

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("The name of the template to include is required.");


            var template = Razorizer.FindTemplate(name);
            foreach (var method in template.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public))
            {
                if (typeof(LambdaWriter).IsAssignableFrom(method.ReturnType))
                {
                    RegisterHelper(name, template, method);
                }
            }
            template.Model = Model;
            return template.Run();
        }

        protected string ToUrl(IModelReference modelRef, string content = null, bool forceLocal = false, string attributes = null, bool useSelf = true)
        {
            return Model.ToUrl(modelRef, content, forceLocal, attributes, useSelf);
        }


        protected string ToUrl(string id, string content = null, bool forceLocal = false, string attributes = null, bool useSelf = true)
        {
            return Model.ToUrl(id, content, forceLocal, attributes, null, useSelf);
        }

        /// <summary>
        /// Copies the content of the directory.
        /// </summary>
        /// <param name="directory">The directory.</param>
        public void CopyDirectoryContent(string directory)
        {
            @Model.CopyStyleContent(directory);
        }

        /// <summary>
        /// Resolves a file from template directories.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>The path to the file</returns>
        public string ResolveFile(string file)
        {
            return Model.ResolvePath(file);
        }

        /// <summary>
        /// Resolves and load a file from template directories.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>The content of the file</returns>
        public string Loadfile(string file)
        {
            return Model.Loadfile(file);
        }

        /// <summary>
        /// Perform regular expression expansion.
        /// </summary>
        /// <param name="content">The content to replace.</param>
        /// <param name="isOnlyForHtmlContent">if set to <c>true</c> [is only for HTML content].</param>
        /// <returns>The content replaced</returns>
        public string TagExpand(string content, bool isOnlyForHtmlContent = false)
        {
            return Model.TagExpand(content, isOnlyForHtmlContent);
        }

        /// <summary>
        /// Parses the specified template name.
        /// </summary>
        /// <param name="templateName">Name of the template.</param>
        /// <returns></returns>
        public string Parse(string templateName)
        {
            return Model.Parse(templateName);
        }

        /// <summary>
        /// Escapes the specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        public string Escape(string content)
        {
            if (content == null)
                return "NULLERROR";
            return Utility.EscapeHtml(content);
        }

        /// <summary>
        /// Escapes the specified content for the Syntax Box (bracket are echaped in &lt; and &gt;) .
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        public string EscapeForSyntaxBox(string content)
        {
            if (content == null)
                return "NULLERROR";
            return Utility.EscapeHtml(content).Replace("&lt;", "<").Replace("&gt;", ">");
        }
    }
}