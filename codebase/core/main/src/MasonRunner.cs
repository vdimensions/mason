using System;
using Axle;

namespace Mason
{
    public static class MasonRunner
    {
        public static void Run(params string[] args)
        {
            Application.Build().Run(args);
        }
    }
}
