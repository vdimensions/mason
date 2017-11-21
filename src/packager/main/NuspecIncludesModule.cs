using System.IO;
using System.Text;
using System.Xml.Linq;

using Mason.Packaging;
using Mason.Sdk;


namespace Mason
{
    sealed class NuspecIncludesModule : AbstractMasonModule<NuspecIncludesSettings>
    {
        private const string PackageIncludeFile = "mason.nuspec-includes.txt";

        internal const string ModuleName = "incl";

        public override NuspecIncludesSettings CreateConfiguration(IMasonProperties properties)
        {
            var augmentedProperties = new ContextProperties
            {
                [NuspecIncludesSettings.Properties.NuspecFile] = "${id}.nuspec"
            };
            return new NuspecIncludesSettings(new PropertiesChain(augmentedProperties, properties));
        }

        public override void Run(NuspecIncludesSettings settings, Options.IOptionMap options)
        {
            PackageContents includes = null;
            var projectPackageIncludes = Path.Combine(settings.Location, $"{settings.ProjectFile}.{PackageIncludeFile}");
            var defaultPackageIncludes = Path.Combine(settings.Location, PackageIncludeFile);
            if (File.Exists(projectPackageIncludes))
            {
                includes = new PackageContents(projectPackageIncludes);
            }
            else if (File.Exists(defaultPackageIncludes))
            {
                includes = new PackageContents(defaultPackageIncludes);
            }
            if (includes != null)
            {
                var nuspec = Path.Combine(settings.Location, settings.NuspecFile);
                if (!File.Exists(nuspec))
                {
                    return;
                }
                XDocument doc;
                using (var reader = new StreamReader(nuspec, Encoding.UTF8))
                {
                    doc = XDocument.Load(reader);
                }
                if (doc?.Root == null)
                {
                    return;
                }
                var filesElement = doc.Root.Element(XName.Get("files"));
                if (filesElement == null)
                {
                    doc.Root.Add(new XElement("files"));
                    filesElement = doc.Root.Element(XName.Get("files"));
                }
                foreach (var packageInclude in includes)
                {
                    if (settings.ExcludeMissingFiles && !File.Exists(Path.Combine(settings.Location, packageInclude.Source)))
                    {
                        continue;
                    }
                    var includeElement = new XElement(
                        "file",
                        new XAttribute("src", Expander.Expand(settings, packageInclude.Source)),
                        new XAttribute("target", Expander.Expand(settings, packageInclude.Destination)));
                    filesElement.Add(includeElement);
                }

                using (var writer = new StreamWriter(nuspec, false, Encoding.UTF8))
                {
                    doc.Save(writer);
                }
            }
        }

        public override string Name => ModuleName;
    }
}