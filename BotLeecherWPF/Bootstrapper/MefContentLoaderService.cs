using BotLeecher.Tools;
using FirstFloor.ModernUI.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotLeecherWPF.Bootstrapper
{
    /// <summary>
    /// Loads content provided by MEF.
    /// </summary>
    [Export]
    public class MefContentLoaderService
        : DefaultContentLoader
    {
        /// <summary>
        /// Allow recomposition to be able to load Content from other Assembly
        /// </summary>
        [ImportMany(AllowRecomposition = true)]
        private Lazy<IContent, IContentMetadata>[] Contents { get; set; }

        [Import]
        private ILogger _logger;

        protected override object LoadContent(Uri uri)
        {
            // lookup the content based on the content uri in the content metadata
            var content = (from c in this.Contents
                           where c.Metadata.ContentUri == GetPageName(uri)
                           select c.Value).FirstOrDefault();

            if (content == null)
            {
                throw new ArgumentException("Invalid uri: " + uri);
            }

            return content;
        }

        private string GetPageName(Uri uri)
        {
            var path = uri.OriginalString;
            if (path.Contains('?'))
            {
                path = path.Substring(0, path.IndexOf('?'));
            }

            return path;
        }
    }
}
