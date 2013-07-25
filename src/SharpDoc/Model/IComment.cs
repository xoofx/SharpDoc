﻿// Copyright (c) 2010-2013 SharpDoc - Alexandre Mutel
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

using System.Xml;

namespace SharpDoc.Model
{
    /// <summary>
    /// This interface contains xml code comments.
    /// </summary>
    public interface IComment
    {
        /// <summary>
        /// Gets or sets the <see cref="XmlNode"/> extracted from the code comments 
        /// for a particular member.
        /// </summary>
        /// <value>The XmlNode doc.</value>
        XmlNode DocNode { get; set; }

        /// <summary>
        /// Gets or sets the description extracted from the &lt;summary&gt; tag of the <see cref="DocNode"/>.
        /// </summary>
        /// <value>The description.</value>
        string Description { get; set; }

        /// <summary>
        /// Gets or sets the remarks extracted from the &lt;remarks&gt; tag of the <see cref="DocNode"/>.
        /// </summary>
        /// <value>The remarks.</value>
        string Remarks { get; set; }

        /// <summary>
        /// Gets or sets the web documentation page extracted from the &lt;webdoc&gt; tag of the <see cref="IComment.DocNode"/>.
        /// </summary>
        /// <value>The corresponding webDocumentation page.</value>
        XmlNode WebDocPage { get; set; }

        /// <summary>
        /// Gets or sets the &lt;inheritdoc&gt; tag of the <see cref="IComment.DocNode"/>.
        /// </summary>
        /// <value>The inheritdoc xml node.</value>
        XmlNode InheritDoc { get; set; }
    }
}