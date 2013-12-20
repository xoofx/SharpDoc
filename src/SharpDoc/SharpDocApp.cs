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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;

using HtmlAgilityPack;
using MarkdownSharp;
using Mono.Options;

using SharpDoc.Logging;
using SharpDoc.Model;
using SharpDocPak;
using SharpRazor;

namespace SharpDoc
{
    /// <summary>
    /// SharpDoc application.
    /// </summary>
    public class SharpDocApp
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SharpDocApp"/> class.
        /// </summary>
        public SharpDocApp()
        {
            Config = new Config();
            StyleManager = new StyleManager();
        }

        /// <summary>
        /// Gets or sets the config.
        /// </summary>
        /// <value>The config.</value>
        public Config Config { get; set; }

        /// <summary>
        /// Gets or sets the style manager.
        /// </summary>
        /// <value>The style manager.</value>
        public StyleManager StyleManager { get; set; }

        /// <summary>
        /// Print usages the error.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <param name="parameters">The parameters.</param>
        private static void UsageError(string error, params object[] parameters)
        {
            var exeName = Path.GetFileName(Assembly.GetEntryAssembly().Location);
            Console.Write("{0}: ", exeName);
            Console.WriteLine(error, parameters);
            Console.WriteLine("Use {0} --help' for more information.", exeName);
            Environment.Exit(1);
        }

        private string TopicLoader(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Logger.Warning("Topic file [{0}] was not found", filePath);
                return null;
            }

            if (filePath.EndsWith(".htm") || filePath.EndsWith(".html"))
            {
                return File.ReadAllText(filePath);
            }

            if (filePath.EndsWith(".md"))
            {

                var markdown = new Markdown();
                return markdown.Transform(File.ReadAllText(filePath));
            }

            Logger.Warning("Template loading failed for file [{0}]. Extension is not supported: Only .htm, .html, .md", Path.GetFileName(filePath));
            return null;
        }

        /// <summary>
        /// Parses the command line arguments.
        /// </summary>
        /// <param name="args">The args.</param>
        public void ParseArguments(string[] args)
        {
            var showHelp = false;

            var files = new List<string>();

            var configParams = new List<ConfigParam>();
            var styleParams = new List<ConfigParam>();
            string webDocumentationUrl = null;
            NetworkCredential webDocumentationLogin = null;

            var options = new OptionSet()
                              {
                                  "Copyright (c) 2010-2013 SharpDoc - Alexandre Mutel",
                                  "Usage: SharpDoc [options]* [--config file.xml | Assembly1.dll Assembly1.xml...]*",
                                  "Documentation generator for .Net languages",
                                  "",
                                  "options:",
                                  {"c|config=", "Configuration file", opt => Config = Config.Load(opt, TopicLoader)},

                                  {
                                      "D=", "Define a template parameter with an (optional) value.",
                                      (param, value) =>
                                          {
                                              if (param == null)
                                                  throw new OptionException("Missing parameter name for option -D.", "-D");
                                              configParams.Add(new ConfigParam(param, value));
                                          }
                                      },
                                  {
                                      "S=", "Define a style parameter with a (optional) value.",
                                      (style, value) =>
                                          {
                                              if (style == null)
                                                  throw new OptionException("Missing parameter name/value for option -S.", "-S");
                                              styleParams.Add(new ConfigParam(style, value));
                                          }
                                      },
                                  {"d|style-dir=", "Add a style directory", opt => Config.StyleDirectories.Add(opt) },
                                  {"s|style=", "Specify the style to use [default: Standard]", opt => Config.StyleNames.Add(opt)},
                                  {"o|output=", "Specify the output directory [default: Output]", opt => Config.OutputDirectory = opt},
                                  {"r|references=", "Add reference assemblies in order to load source assemblies", opt => Config.References.Add(opt)},
                                  {"w|webdoc=", "Url of the extern documentation site [with the protocol to use, ex: http(s)://...]", 
                                      (protocol, domain) =>
                                        {
                                            if (protocol == null || domain == null)
                                                throw new OptionException("Missing parameter web site home page url for option -w.", "-w");
                                            webDocumentationUrl = WebDocumentation.BuildWebDocumentationUrl(protocol, domain);
                                        }
                                      },
                                  {"wL|webdocLogin=", "(optional) Authentification file for the extern documentation site (first line: username, second line: password)", 
                                      opt =>
                                        {
                                            if (opt == null)
                                                throw new OptionException("Missing parameter web site auth file for option -wL.", "-wL");
                                            if (!File.Exists(opt))
                                                throw new OptionException("Auth config file doesn't exist.", "-wL");
                                            var lines = File.ReadAllLines(opt);
                                            if (lines.Length < 2)
                                                throw new OptionException("Invalid auth config file, should be one line for username, one line for password", "-wL");
                                            webDocumentationLogin = new NetworkCredential(lines[0], lines[1]);
                                        }
                                      },
                                  "",
                                  {"h|help", "Show this message and exit", opt => showHelp = opt != null},
                                  "",
                                  "[Assembly1.dll Assembly1.xml...] Source files, if a config file is not specified, load source assembly and xml from the specified list of files",
                                  // default
                                  {"<>", opt => files.AddRange(opt.Split(' ', '\t')) },
                              };           
            try
            {
                options.Parse(args);

                StyleManager.Init(Config);
            }
            catch (OptionException e)
            {
                UsageError(e.Message);
            }

            if (showHelp)
            {
                options.WriteOptionDescriptions(Console.Out);
                StyleManager.WriteAvailaibleStyles(Console.Out);
                Environment.Exit(0);
            }

            // Copy config params from command line to current config
            Config.Parameters.AddRange(configParams);
            Config.StyleParameters.AddRange(styleParams);

            // Override webdoc url from commmand line parameters
            if (webDocumentationUrl != null)
            {
                Config.WebDocumentationUrl = webDocumentationUrl;
            }
            if (webDocumentationLogin != null)
            {
                Config.WebDocumentationLogin = webDocumentationLogin;
            }

            // Add files from command line
            if (files.Count > 0)
            {
                foreach (var file in files)
                {
                    var configSource = new ConfigSource();
                    var ext = Path.GetExtension(file);
                    if (ext != null && ext.ToLower() == ".xml")
                    {
                        configSource.DocumentationPath = file;
                    }
                    else
                    {
                        configSource.AssemblyPath = file;
                    }

                    Config.Sources.Add(configSource);
                }
            }

            if (Config.Sources.Count == 0)
                UsageError("At least one option is missing. Either a valid config file (-config) or a direct list of assembly/xml files must be specified");

            // Add default style Standard if none is defined
            if (Config.StyleNames.Count == 0)
                Config.StyleNames.Add("Standard");

            // Verify the validity of the style
            foreach (var styleName in Config.StyleNames)
            {
                if (!StyleManager.StyleExist(styleName))
                    UsageError("Style [{0}] does not exist. Use --help to have a list of available styles.", styleName);
            }
        }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        public void Run()
        {
            var clock = Stopwatch.StartNew();

            var razorizer = new Razorizer(typeof (PageTemplateDoc)) { EnableDebug = true };

            // New instance of a template context used by the RazorEngine
            var context = new TemplateContext(razorizer)
            {
                Config = Config,
                StyleManager = StyleManager,
            };

            // Create web documentation
            if (Config.WebDocumentationUrl != null)
            {
                context.WebDocumentation = new WebDocumentation(Config.WebDocumentationUrl, Config.WebDocumentationLogin);
            }

            // Setup the context based on the config and StyleManager
            context.Initialize();

            // Verify the validity of the style
            foreach (var styleName in Config.StyleNames)
            {
                Logger.Message("-------------------------------------------------------------------------------");
                Logger.Message("Generating documentation using [{0}] style", styleName);
                Logger.Message("-------------------------------------------------------------------------------");
                context.UseStyle(styleName);
                context.Parse(StyleDefinition.DefaultBootableTemplateName);
            }

            Logger.Message("Total time: {0:F1}s", clock.ElapsedMilliseconds / 1000.0f);
            //Logger.Message("Time for assembly processing: {0:F1}s", timeForModelProcessor/1000.0f);
            //Logger.Message("Time for writing content: {0:F1}s", timeForWriting/1000.0f);

            if ((Config.OutputType & OutputType.DocPak) != 0 )
                GenerateDocPak();
        }

        private void GenerateDocPak()
        {
            if (Config.DocPak == null)
            {
                Logger.Error("Docpak config not found from the config file. Cannot generate docpak");
                return;
            }

            var sharpDocPak = new SharpDocPakApp
                                  {
                                      Output = Config.DocPak.Name,
                                      CommandType = CommandType.Pak,
                                      DirectoryLocation = Config.AbsoluteOutputDirectory,
                                      Tags = Config.DocPak.Tags,
                                      Title = Config.Title
                                  };

            sharpDocPak.Run();
        }

        static public string[] GetFiles(string[] patterns)
        {
            List<string> filelist = new List<string>();
            foreach (string pattern in patterns)
                filelist.AddRange(GetFiles(pattern));
            string[] files = new string[filelist.Count];
            filelist.CopyTo(files, 0);
            return files;
        }

        static public string[] GetFiles(string patternlist)
        {
            List<string> filelist = new List<string>();
            foreach (string pattern in
                patternlist.Split(Path.GetInvalidPathChars()))
            {
                string dir = Path.GetDirectoryName(pattern);
                if (String.IsNullOrEmpty(dir)) dir =
                     Directory.GetCurrentDirectory();
                filelist.AddRange(Directory.GetFiles(
                    Path.GetFullPath(dir),
                    Path.GetFileName(pattern)));
            }
            string[] files = new string[filelist.Count];
            filelist.CopyTo(files, 0);
            return files;
        }
    }
}