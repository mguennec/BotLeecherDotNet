using FirstFloor.ModernUI.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFGenerics
{
    /// <summary>
    /// http://mui.codeplex.com/wikipage?title=A%20guide%20to%20using%20MEF
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ContentAttribute
        : ExportAttribute
    {
        public ContentAttribute(string contentUri)
            : base(typeof(IContent))
        {
            this.ContentUri = contentUri;
        }
        public string ContentUri { get; private set; }
    }
}
