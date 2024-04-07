using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.NuGet.Push;
using Cake.Frosting;

namespace BuildScripts;

[TaskName(nameof(DeployToNuGetTask))]
public sealed class DeployToNuGetTask : FrostingTask<BuildContext>
{
    public override bool ShouldRun(BuildContext context)
    {
        return context.IsRunningOnGitHubActions &&
               context.IsTag &&
               context.RepositoryOwner == "craftworksgames";
    }

    public override void Run(BuildContext context)
    {
        DotNetNuGetPushSettings pushSettings = new DotNetNuGetPushSettings()
        {
            Source = $"https://api.nuget.org/v3/index.json",
            ApiKey = context.NuGetAccessToken
        };

        context.DotNetNuGetPush("artifacts/*.nupkg", pushSettings);
    }
}
