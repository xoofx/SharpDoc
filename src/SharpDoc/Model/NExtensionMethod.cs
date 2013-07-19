// Copyright (c) 2010-2013 SharpDX - Alexandre Mutel
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
using System.Text;
using System.Collections.Generic;

namespace SharpDoc.Model
{
    /// <summary>
    /// An extension method description.
    /// </summary>
    public class NExtensionMethod
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NExtensionMethod"/> class.
        /// </summary>
        public NExtensionMethod(NTypeReference extendedType, NMethod extensionMethod)
        {
            ExtendedType = extendedType.ElementType;

            Method = new NMethod();
            Method.Assembly = extensionMethod.Assembly;
            Method.Description = extensionMethod.Description;
            Method.DocNode = extensionMethod.DocNode;
            Method.ElementType = extensionMethod.ElementType;
            Method.FullName = extensionMethod.FullName;
            Method.GenericArguments = extensionMethod.GenericArguments;
            Method.GenericParameters = extensionMethod.GenericParameters;
            Method.HasOverrides = extensionMethod.HasOverrides;
            Method.Id = extensionMethod.Id;
            Method.Implements = extensionMethod.Implements;
            Method.IsAbstract = extensionMethod.IsAbstract;
            Method.IsObsolete = extensionMethod.IsObsolete;
            Method.IsVirtual = extensionMethod.IsVirtual;
            Method.IsObsolete = extensionMethod.IsObsolete;
            Method.MsdnId = extensionMethod.MsdnId;
            Method.Name = extensionMethod.Name;
            Method.ObsoleteMessage = extensionMethod.ObsoleteMessage;
            Method.PageId = extensionMethod.PageId;
            Method.PageTitle = extensionMethod.PageTitle;
            Method.Remarks = extensionMethod.Remarks;
            Method.SeeAlsos = extensionMethod.SeeAlsos;
            Method.TopicLink = extensionMethod.TopicLink;
            Method.UnManagedApi = extensionMethod.UnManagedApi;
            Method.UnManagedShortApi = extensionMethod.UnManagedShortApi;
            Method.Namespace = extensionMethod.Namespace;
            Method.Visibility = extensionMethod.Visibility;
            Method.ReturnDescription = extensionMethod.ReturnDescription;
            Method.ReturnType = extensionMethod.ReturnType;
            Method.MemberType = extensionMethod.MemberType;
            foreach (var group in extensionMethod.Groups)
                Method.SetApiGroup(group, true);

            // The extension method as member method is not static
            Method.IsStatic = false;


            Method.MemberType = NMemberType.Extension;
            Method.IsExtensionMethod = true;
            Method.DeclaringType = extendedType.ElementType;
            Method.ExtensionSource = extensionMethod.DeclaringType;

            
            // Remove the "this parameter"
            Method.Parameters = new List<NParameter>(extensionMethod.Parameters);
            Method.Parameters.RemoveAt(0);

            // Rebuild the signature without the "this parameter"
            var signature = new StringBuilder();
            signature.Append(Method.Name);
            MonoCecilModelBuilder.BuildMethodSignatureParameters(Method, signature);
            Method.Signature = signature.ToString();
        }

        /// <summary>
        /// Gets or sets the type concerned by the extension.
        /// </summary>
        public NTypeReference ExtendedType { get; set; }

        /// <summary>
        /// Gets or sets the extension method.
        /// </summary>
        public NMethod Method { get; set; }
    }
}