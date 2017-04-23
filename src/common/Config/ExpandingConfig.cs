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
            Expander expander = new Expander();
            IBuildConfig cfg = new BuildConfigChain(this, rawConfig);
            foreach (string key in rawConfig.Keys)
            {
                string expandedValue = expander.Expand(cfg, rawConfig[key]);
                this[key] = expandedValue;
            }
        }
    }
}

