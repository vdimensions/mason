using System;


namespace Mason.Config
{
    internal class EnvironmentConfig : AbstractBuildConfig
    {
        private readonly EnvironmentVariableTarget target;

        public EnvironmentConfig(EnvironmentVariableTarget target)
        {
            this.target = target;
            Read( );
        }

        protected sealed override void Read()
        {
            var env = Environment.GetEnvironmentVariables(target);
            foreach (string key in env.Keys)
            {
                this[key] = (string) env[key];
            }
        }
    }
}
