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

namespace SharpDoc.Model
{
    /// <summary>
    /// An event documentation.
    /// </summary>
    public class NEvent : NMember, IOverridable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NEvent"/> class.
        /// </summary>
        public NEvent()
        {
            Category = "Event";
        }

        /// <summary>
        /// Gets or sets the type of the event.
        /// </summary>
        /// <value>
        /// The type of the event.
        /// </value>
        public INTypeReference EventType { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is virtual.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is virtual; otherwise, <c>false</c>.
        /// </value>
        public bool IsVirtual { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance has overrides.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has overrides; otherwise, <c>false</c>.
        /// </value>
        public bool HasOverrides
        {
            get
            {
                return Overrides != null;
            }
        }

        /// <summary>
        /// Gets or sets the overrides.
        /// </summary>
        /// <value>The overrides.</value>
        public INMemberReference Overrides { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance has implements.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has implements; otherwise, <c>false</c>.
        /// </value>
        public bool HasImplements
        {
            get
            {
                return Implements != null;
            }
        }

        /// <summary>
        /// Gets or sets the implements.
        /// </summary>
        /// <value>The implements.</value>
        public INMemberReference Implements { get; set; }

        public override void CopyDocumentation(INMemberReference other)
        {
            base.CopyDocumentation(other);
            var otherEvent = other as NEvent;
            if (otherEvent != null)
            {
                if (string.IsNullOrEmpty(ObsoleteMessage) && !string.IsNullOrEmpty(otherEvent.ObsoleteMessage))
                    ObsoleteMessage = otherEvent.ObsoleteMessage;
            }
        }
    }
}