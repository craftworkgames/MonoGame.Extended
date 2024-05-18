using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Build;
using Cake.Common.Tools.DotNet.MSBuild;
using Cake.Frosting;

namespace BuildScripts;

[TaskName(nameof(BuildTask))]
public sealed class BuildTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        DotNetMSBuildSettings msBuildSettings = new DotNetMSBuildSettings();
        msBuildSettings.WithProperty("Version", context.Version);
        msBuildSettings.WithProperty("NoWarn", "CS1591");


        DotNetBuildSettings buildSettings = new DotNetBuildSettings()
        {
            MSBuildSettings = msBuildSettings,
            Configuration = "Release",
            Verbosity = DotNetVerbosity.Minimal,
            NoRestore = true,
            NoLogo = true
        };

        context.DotNetBuild(context.SolutionPath, buildSettings);
    }
}
