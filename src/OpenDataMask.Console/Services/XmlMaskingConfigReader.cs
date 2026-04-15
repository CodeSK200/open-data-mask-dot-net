using System.Collections.Immutable;
using System.Linq;
using System.Xml.Linq;
using OpenDataMask.Console.Models;

namespace OpenDataMask.Console.Services
{
    public sealed class XmlMaskingConfigReader : IMaskingConfigReader
    {
        public MaskingConfig Read(string path)
        {
            var document = XDocument.Load(path);
            if (document.Root is null)
            {
                throw new InvalidOperationException("Masking configuration XML must have a root element.");
            }

            var collections = document.Root.Elements().ToImmutableDictionary(
                element => element.Name.LocalName,
                element => element.Elements().Select(child => child.Name.LocalName).ToList().AsReadOnly() as IReadOnlyList<string>);

            return new MaskingConfig(collections);
        }
    }
}
