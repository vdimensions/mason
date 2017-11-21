using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Mason.Processing;
using Mason.Sdk;


namespace Mason
{
    sealed class PreprocessorModule : AbstractMasonModule<PreprocessorSettings>
    {
        public override PreprocessorSettings CreateConfiguration(IMasonProperties properties)
        {
            var augmentedProperties = new ContextProperties
            {
                [PreprocessorSettings.Properties.TemplateFileMatchPattern] = "*.template",
                [PreprocessorSettings.Properties.TemplateFileEncoding] = "UTF-8"
            };
            return new PreprocessorSettings(new PropertiesChain(properties, augmentedProperties));
        }

        public override void Run(PreprocessorSettings settings, Options.IOptionMap options)
        {
            // TODO: check if different location is passed as an argument

            var files = new DirectoryInfo(settings.Location).GetFiles(
                settings.TemplateFilePattern,
                SearchOption.AllDirectories);

            var templateExtension = settings.TemplateFilePattern.Substring(settings.TemplateFilePattern.LastIndexOf('.'));

            IList<FileReplacementPair> result = new List<FileReplacementPair>( );
            foreach (var rawFile in files.Where(f => f.FullName.EndsWith(templateExtension, StringComparison.Ordinal)))
            {
                //var extIndex = rawFile.FullName.LastIndexOf(templateExtension, StringComparison.Ordinal);
                var targetFile = new FileInfo(rawFile.FullName.Substring(0, rawFile.FullName.Length - (templateExtension.Length + 1)));
                result.Add(new FileReplacementPair(rawFile, targetFile));
            }

            foreach (var pair in result)
            {
                var replacedContents = Expander.Expand(settings, File.ReadAllText(pair.RawFile.FullName, settings.TemplateFileEncoding));
                using (var fileStream = new FileStream(pair.TargetFile.FullName, FileMode.Create, FileAccess.Write))
                using (var writer = new StreamWriter(fileStream, settings.TemplateFileEncoding))
                {
                    writer.Write(replacedContents);
                    writer.Flush();
                    fileStream.Flush();
                }
            }
        }

        public override string Name => "preprocess";
    }
}
