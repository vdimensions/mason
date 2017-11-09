using Mason.Sdk;


namespace Mason.Packaging
{
    public class PackagingSettings : AbstractSettings
    {
        public static class Properties
        {
            public const string OutputLocation       = "mason-packager.output.location";
        }
        public PackagingSettings(IMasonProperties properties) : base(properties)
        {
            OutputLocation = properties[Properties.OutputLocation];
        }
        public string OutputLocation { get; }
    }
}