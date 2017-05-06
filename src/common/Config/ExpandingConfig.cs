namespace Mason.Config
{
    public class ExpandingConfig : AbstractBuildConfig
    {
        private readonly IBuildConfig rawConfig;

        public ExpandingConfig (IBuildConfig raw)
        {
            this.rawConfig = raw;
            Read( );
        }

        protected override void Read( )
        {
            var expander = new Expander();
            IBuildConfig cfg = new BuildConfigChain(this, rawConfig);
            foreach (var key in rawConfig.Keys)
            {
                var expandedValue = expander.Expand(cfg, rawConfig[key]);
                this[key] = expandedValue;
            }
        }
    }
}

