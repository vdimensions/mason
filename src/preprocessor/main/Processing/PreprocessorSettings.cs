using System.Collections.Generic;
using System.Text;

using Mason.Sdk;


namespace Mason.Processing
{
    public class PreprocessorSettings : AbstractMasonSettings, IMasonProperties
    {
        public static class Properties
        {
            public const string TemplateFileMatchPattern = "mason.preprocessor.template-file-pattern";
            public const string TemplateFileEncoding     = "mason.preprocessor.template-file-encoding";
        }

        private readonly IMasonProperties _properties;

        public PreprocessorSettings(IMasonProperties properties) : base(properties)
        {
            _properties = properties;
            TemplateFilePattern = properties[Properties.TemplateFileMatchPattern];
            var enc = properties[Properties.TemplateFileEncoding];
            if (!string.IsNullOrEmpty(enc))
            {
                TemplateFileEncoding = Encoding.GetEncoding(enc);
            }
        }
        public string TemplateFilePattern { get; }
        public Encoding TemplateFileEncoding { get; }

        IEnumerable<string> IMasonProperties.Keys => _properties.Keys;
        string IMasonProperties.this[string property] => _properties[property];
    }
}