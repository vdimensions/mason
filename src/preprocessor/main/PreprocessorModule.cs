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
        private const string ModuleName = "prep";

        public override PreprocessorSettings CreateConfiguration(IMasonProperties properties)
        {
            var augmentedProperties = new DefaultProperties
            {
                [PreprocessorSettings.Properties.TemplateFileExtension] = PreprocessorSettings.Defaults.TemplateFileExtension,
                [PreprocessorSettings.Properties.TemplateFileEncoding] = PreprocessorSettings.Defaults.TemplateFileEncoding
            };
            return new PreprocessorSettings(new PropertiesChain(properties, augmentedProperties));
        }

        public override void Run(PreprocessorSettings settings, Options.IOptionMap options)
        {
            var files = new DirectoryInfo(settings.ProjectDir).GetFiles(
                settings.TemplateFilePattern,
                SearchOption.AllDirectories);

            Console.WriteLine($"Looking up directory {settings.ProjectDir} for {settings.TemplateFilePattern}, found {files.Length} matches");

            var templateExtension = settings.TemplateFileExtension;

            IList<FileReplacementPair> result = new List<FileReplacementPair>();
            foreach (var rawFile in files.Where(f => f.FullName.EndsWith(templateExtension, StringComparison.OrdinalIgnoreCase)))
            {
                var targetFile = new FileInfo(rawFile.FullName.Substring(0, rawFile.FullName.Length - templateExtension.Length));
                result.Add(new FileReplacementPair(rawFile, targetFile));
            }

            foreach (var pair in result)
            {
                Console.WriteLine($"Preprocessing file {pair.TargetFile.FullName} using template {pair.RawFile.FullName}");
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

        public override string Name => ModuleName;
    }
}
