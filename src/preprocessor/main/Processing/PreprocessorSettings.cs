using System.Collections.Generic;
using System.Text;

using Mason.Sdk;


namespace Mason.Processing
{
    public class PreprocessorSettings : AbstractMasonSettings, IMasonProperties
    {
        public static class Properties
        {
            public const string TemplateFileExtension = "mason.preprocessor.template-file-extension";
            public const string TemplateFileEncoding  = "mason.preprocessor.template-file-encoding";
        }
        public static class Defaults
        {
            public const string TemplateFileExtension = ".template";
            public const string TemplateFileEncoding  = "UTF-8";
        }

        private readonly IMasonProperties _properties;

        public PreprocessorSettings(IMasonProperties properties) : base(properties)
        {
            _properties = properties;
            TemplateFileExtension = string.Format($".{properties[Properties.TemplateFileExtension].TrimStart('.')}");
            TemplateFilePattern = string.Format("*{0}", TemplateFileExtension);
            var enc = properties[Properties.TemplateFileEncoding];
            if (!string.IsNullOrEmpty(enc))
            {
                TemplateFileEncoding = Encoding.GetEncoding(enc);
            }
        }
        public string TemplateFileExtension { get; }
        public string TemplateFilePattern { get; }
        public Encoding TemplateFileEncoding { get; }

        IEnumerable<string> IMasonProperties.Keys => _properties.Keys;
        string IMasonProperties.this[string property] => _properties[property];
    }
}