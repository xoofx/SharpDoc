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
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using SharpDoc.Model;

namespace SharpDoc
{
    /// <summary>
    /// Config file for SharpDoc.
    /// </summary>
    [XmlRoot("config", Namespace = NS)]
    public class Config
    {
        internal const string NS = "SharpDoc";

        /// <summary>
        /// Initializes a new instance of the <see cref="Config"/> class.
        /// </summary>
        public Config()
        {
            StyleNames = new List<string>();
            OutputDirectory = "Output";
            Sources = new List<ConfigSource>();
            References = new List<string>();
            Parameters = new List<ConfigParam>();
            ExcludeList = new List<string>();
            StyleParameters = new List<ConfigParam>();
            StyleDirectories = new List<string>();
            OutputType = OutputType.Default;

            WebDocumentationUrl = null;
            WebDocumentationLogin = null;
            WebDocumentation = null;
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        [XmlElement("title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        /// <value>The file path.</value>
        [XmlIgnore]
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets the topic.
        /// </summary>
        /// <value>The topic.</value>
        [XmlElement("topic")]
        public NTopic RootTopic { get; set; }

        /// <summary>
        /// Gets or sets the output directory.
        /// </summary>
        /// <value>The output directory.</value>
        [XmlElement("output-dir")]
        public string OutputDirectory { get; set; }

        /// <summary>
        /// Gets the absolute output directory.
        /// </summary>
        /// <value>The absolute output directory.</value>
        public string AbsoluteOutputDirectory
        {
            get { return Path.Combine(string.IsNullOrEmpty(FilePath) ? Environment.CurrentDirectory : Path.GetDirectoryName(FilePath), OutputDirectory); }
        }

        /// <summary>
        /// Gets or sets a list of source file (assembly or xml comment file).
        /// </summary>
        /// <value>The sources file.</value>
        [XmlElement("source")]
        public List<ConfigSource> Sources { get; set; }

        /// <summary>
        /// Gets or sets a list assembly references.
        /// </summary>
        /// <value>The references.</value>
        [XmlElement("reference")]
        public List<string> References { get; set; }

        /// <summary>
        /// Gets or sets the extern documentation webSite connection.
        /// </summary>
        /// <value>The the extern documentation webSite connection.</value>
        //[XmlElement("webDocumentation")]
        [XmlIgnore]
        public WebDocumentation WebDocumentation { get; set; }

        /// <summary>
        /// Gets or sets the extern documentation webSite url.
        /// </summary>
        /// <value>The extern documentation webSite url.</value>
        //[XmlElement("webDocumentationUrl")]
        [XmlIgnore]
        public string WebDocumentationUrl { get; set; }

        /// <summary>
        /// Gets or sets the extern documentation webSite login.
        /// </summary>
        /// <value>The the extern documentation webSite login.</value>
        //[XmlElement("webDocumentationLogin")]
        [XmlIgnore]
        public NetworkCredential WebDocumentationLogin { get; set; }


        /// <summary>
        /// Gets or sets parameters.
        /// </summary>
        /// <value>The parameters.</value>
        [XmlElement("param")]
        public List<ConfigParam> Parameters { get; set; }

        /// <summary>
        /// Gets or sets styles override.
        /// </summary>
        /// <value>The styles override.</value>
        [XmlElement("param-style")]
        public List<ConfigParam> StyleParameters { get; set; }

        /// <summary>
        /// Gets or sets the name of the style.
        /// </summary>
        /// <value>The name of the style.</value>
        [XmlElement("style")]
        public List<string> StyleNames { get; set; }

        /// <summary>
        /// Gets or sets the exclude list (namespaces, types...etc.).
        /// </summary>
        /// <value>The exclude list.</value>
        [XmlElement("exclude")]
        public List<string> ExcludeList { get; set; }

        /// <summary>
        /// Gets or sets styles override.
        /// </summary>
        /// <value>The styles override.</value>
        [XmlElement("style-dir")]
        public List<string> StyleDirectories { get; set; }

        /// <summary>
        /// Style directory name
        /// </summary>
        public const string DefaultStyleDirectoryName = "Styles";

        /// <summary>
        /// Gets or sets the type of the output.
        /// </summary>
        /// <value>The type of the output.</value>
        [XmlElement("output-type")]
        public OutputType OutputType { get; set; }

        /// <summary>
        /// Gets or sets the doc pak.
        /// </summary>
        /// <value>The doc pak.</value>
        //[XmlElement("docpak")]
        [XmlIgnore]
        public ConfigDocPak DocPak { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [use template doc].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use template doc]; otherwise, <c>false</c>.
        /// </value>
        [XmlElement("topic-template")]
        public string TopicTemplate { get; set; }

        /// <summary>
        /// Loads the specified config file.
        /// </summary>
        /// <param name="file">The config file.</param>
        /// <returns></returns>
        public static Config Load(string file, TopicContentLoaderDelegate contentLoader)
        {
            var deserializer = new XmlSerializer(typeof(Config));
            var config = (Config)deserializer.Deserialize(new StringReader(File.ReadAllText(file)));
            config.FilePath = file;

            // Update Config for all topics
            if (config.RootTopic != null)
            {
                config.RootTopic.ForEachTopic(topic => topic.Config = config);
                config.RootTopic.Init(contentLoader);
            }

            return config;
        }

        /// <summary>
        /// Finds the topic by id.
        /// </summary>
        /// <param name="topicId">The topic id.</param>
        /// <returns></returns>
        public NTopic FindTopicById(string topicId)
        {
            if (RootTopic == null)
                return null;
            return RootTopic.FindTopicById(topicId);
        }

        /// <summary>
        /// Loads the specified config file.
        /// </summary>
        /// <param name="file">The config file.</param>
        /// <returns></returns>
        public void Save(string file)
        {
            var ns = new XmlSerializerNamespaces();
            ns.Add("", NS);
            var deserializer = new XmlSerializer(typeof(Config));
            var output = new FileStream(file, FileMode.Create);
            deserializer.Serialize(output, this, ns);
        }

        public void InitializeWebDocumentation()
        {
            if (WebDocumentationUrl != null)
            {
                WebDocumentation = new WebDocumentation(WebDocumentationUrl, WebDocumentationLogin);
            }
        }

        public void AddWebDocumentationUrl(string protocol, string domain)
        {
            StringBuilder urlBuilder = new StringBuilder(protocol);
            urlBuilder.Append(":");
            urlBuilder.Append(domain);
            string url = urlBuilder.ToString();

            if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
                WebDocumentationUrl = url;
            else
                throw new Mono.Options.OptionException("Given url is invalid option -w.", "-w");
        }

        public void AddWebDocumentationLogin(string userName, string passWord)
        {
            WebDocumentationLogin = new NetworkCredential(userName, passWord); 
        }
    }
}