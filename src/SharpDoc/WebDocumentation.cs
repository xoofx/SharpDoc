using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using HtmlAgilityPack;
using Mono.Options;

namespace SharpDoc
{
    public class WebDocumentation : WebClient
    {
        private byte[] loginBytes;
        private Uri siteHome;
        private Uri currentUrl;
        private bool siteNeedAuthentification;

        public HtmlDocument currentDocument { get; set; }

        /// <summary>
        /// Gets or sets the function that format credential for the extern documentation site authentification.
        /// </summary>
        /// <value>
        /// The function that format credential for the extern documentation site authentification.
        /// </value>
        public Func<NetworkCredential, string> FormatCredential { get; set; }

        public WebDocumentation(string home, NetworkCredential loginData = null)
        {
            siteHome = new Uri(home);
            currentUrl = siteHome;

            currentDocument = new HtmlDocument();

            if (loginData == null)
                siteNeedAuthentification = false;
            else
            {
                siteNeedAuthentification = true;
                Credentials = loginData;
            }

            if (!Uri.IsWellFormedUriString(home, UriKind.Absolute))
                throw new OptionException("The given web site url for option -w could not be found.", "-w");
        }


        public void Authentify()
        {
            if (siteNeedAuthentification)
            {
                // Format the loginData to match the site log formular
                string login = FormatCredential(Credentials as NetworkCredential);
                loginBytes = Encoding.ASCII.GetBytes(login);
            }
        }

        public bool Load(string page, string siteURL = null)
        {
            currentUrl = (siteURL != null) ? new Uri(siteURL) : siteHome;
            Stream webPage = LoadPage(page, true);
            if (webPage != null)
            {
                currentDocument.Load(webPage, Encoding.UTF8);
                return true;
            }
            else
                return false;
        }

        public Stream LoadPage(string page, bool authentification = true)
        {
            return LoadPage(new Uri( currentUrl, page), authentification);
        }

        public Stream LoadPage(Uri pageUri, bool authentification = true)
        {
            if (pageUri.IsWellFormedOriginalString())
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(pageUri);
                if (authentification && siteNeedAuthentification)
                {
                    request.Method = WebRequestMethods.Http.Post;
                    request.AllowWriteStreamBuffering = true;
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = loginBytes.Length;

                    Stream newStream = request.GetRequestStream();
                    newStream.Write(loginBytes, 0, loginBytes.Length);
                    newStream.Close();
                }

                try
                {
                    return request.GetResponse().GetResponseStream();
                }
                catch (WebException e)
                {
                    if (e.Status == WebExceptionStatus.ProtocolError)
                    {
                        var statusCode = ((HttpWebResponse)e.Response).StatusCode;
                        switch (statusCode)
                        {
                            case HttpStatusCode.MethodNotAllowed:
                                {
                                    // if the authentification failed, try without it
                                    if (authentification == true)
                                        return LoadPage(pageUri, false);
                                    else
                                        throw e;
                                }
                            case HttpStatusCode.NotFound:
                                {
                                    Console.WriteLine("The web documentation page {1} could not be found", pageUri);
                                    return null;
                                }
                            default:
                                throw e;
                        }
                    }
                    else
                        throw e;
                }
            }
            else
            {
                Console.WriteLine("The web documentation page {0} is not a valid Uri", pageUri);
                return null;
            }
        }

        public Uri GetAbsoluteUri(string page)
        {
            return new Uri(currentUrl, page);
        }

        public string GetContentById(string id)
        {
            HtmlNode node = currentDocument.GetElementbyId(id);
            if (node != null)
                return node.InnerHtml;
            else
                return string.Empty;
        }

        public void LimitDocumentToElement(string elementHtml)
        {
            currentDocument.LoadHtml(elementHtml);
        }

        public string GetContent()
        {
            return currentDocument.DocumentNode.InnerHtml;
        }

        public string GetContentByClass(string id, int instanceNumber = 0, string tagType = "div")
        {
            string classRegEx = string.Format("//{1}[@class='{0}']", id, tagType);
            var nodes = currentDocument.DocumentNode.SelectNodes(classRegEx);
            var node = (nodes != null && nodes.Count > instanceNumber) ? nodes[instanceNumber] : null;

            if (node != null)
                return node.InnerHtml;
            else
                return string.Empty;
        }


        public List<string> InternalizeCss(string outDir, string linkToCssDir, string isolationPrefix)
        {
            string cssDir = Path.Combine(Path.Combine(outDir, "html"), linkToCssDir);
            List<string> cssList = new List<string>();

            // select all css defined by link tags with relation = 'stylesheet'
            var nodes = currentDocument.DocumentNode.SelectNodes("//link[@rel='stylesheet']");
            if (nodes == null)
                return new List<string>();

            foreach (var node in nodes)
            {
                string cssUrl =  node.GetAttributeValue("href", string.Empty);

                string newCssName = string.Format("webDocCss_{0}.css", cssUrl.GetHashCode());
                string newCssAboslutePath = Path.Combine(cssDir, newCssName);
                string newCssRelativePath = Path.Combine(linkToCssDir, newCssName);

                cssList.Add(newCssRelativePath);

                if (!File.Exists(newCssAboslutePath))
                {
                    Stream cssStream = LoadPage(cssUrl);
                    if (cssStream != null)
                    {
                        string cssContent = new StreamReader(cssStream).ReadToEnd();
                        cssContent = IsolateCss(cssContent, isolationPrefix);

                        if (!Directory.Exists(cssDir))
                            Directory.CreateDirectory(cssDir);

                        File.WriteAllText(newCssAboslutePath, cssContent);
                    }
                } 
            }

            return cssList;
        }

        public void InternalizeImages(string outDir, string linkToImgDir)
        {
            string imageDir = Path.Combine(Path.Combine(outDir, "html"), linkToImgDir);

            // select all img tags
            var nodes = currentDocument.DocumentNode.SelectNodes("//img");
            if (nodes == null)
                return;

            foreach(var node in nodes)
            {
                string imageUrl = node.GetAttributeValue("src", string.Empty);

                int indexParam = imageUrl.IndexOf('?');
                if (indexParam != -1)
                    imageUrl = imageUrl.Substring(0, indexParam);

                Uri absoluteImageUrl = new Uri(currentUrl, imageUrl);
                string extension = (Path.HasExtension(imageUrl)) ? Path.GetExtension(imageUrl) : ".png";

                string newImageName = string.Format("webDocImage_{0}{1}", imageUrl.GetHashCode(), extension);
                string newImageAboslutePath = Path.Combine(imageDir, newImageName);
                string newImageRelativePath = Path.Combine(linkToImgDir, newImageName);

                if (!File.Exists(newImageAboslutePath))
                {
                    if (!Directory.Exists(imageDir))
                        Directory.CreateDirectory(imageDir);

                    DownloadFile(absoluteImageUrl, newImageAboslutePath);
                }

                node.SetAttributeValue("src", newImageRelativePath);
            }
        }

        private string cssIsolationPrefix;
        private string cssIsolationPrefix2;

        private string IsolateCss(Match match)
        {
            var pattern = match.Groups["pattern"].Value;
            var rules = match.Groups["rules"].Value;
            pattern = pattern.Replace(",", cssIsolationPrefix2);
            return cssIsolationPrefix + pattern + rules;
        }

        private string IsolateCss(string cssContent, string prefix)
        {
            cssIsolationPrefix = prefix + " ";
            cssIsolationPrefix2 = "," + cssIsolationPrefix;

            Regex selectCssRules = new Regex("(?<pattern>[^{]*)(?<rules>{[^}]*})");
            return selectCssRules.Replace(cssContent, IsolateCss);
        }

        public void UseAbsoluteUrls()
        {
            // select all link <a> tags
            var nodes = currentDocument.DocumentNode.SelectNodes("//a[@href]");
            if (nodes == null)
                return;
            foreach (var node in nodes)
            {
                string href = node.GetAttributeValue("href", string.Empty);
                if(!Uri.IsWellFormedUriString(href, UriKind.Absolute))
                {
                    Uri absoluteUri = new Uri(currentUrl, href);
                    node.SetAttributeValue("href", absoluteUri.ToString());
                }
            }

        }
    }
}
