using System.Linq;
using Cake.Common;
using Cake.Common.Build;
using Cake.Common.Xml;
using Cake.Core;
using Cake.Frosting;

namespace BuildScripts;

public sealed class BuildContext : FrostingContext
{
    public string ArtifactsDirectory { get; }
    public string Version { get; }

    public BuildContext(ICakeContext context) : base(context)
    {
        ArtifactsDirectory = context.Arguments(nameof(ArtifactsDirectory), "artifacts").FirstOrDefault();
        Version = context.XmlPeek("Directory.Build.targets", "/Project/PropertyGroup/Version");

        if (context.BuildSystem().IsRunningOnGitHubActions)
        {
            Version += "." + context.EnvironmentVariable("GITHUB_RUN_NUMBER");
        }
    }
}
