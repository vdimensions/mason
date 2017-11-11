using Mason.Sdk;


namespace Mason.Packaging
{
    public class PackagingSettings : AbstractMasonSettings
    {
        public static class Properties
        {
            public const string OutputLocation = "mason-packager.output.location";
        }
        public PackagingSettings(IMasonProperties properties) : base(properties)
        {
            OutputLocation = GetRequiredProperty(Properties.OutputLocation);
        }
        public string OutputLocation { get; }
    }
}