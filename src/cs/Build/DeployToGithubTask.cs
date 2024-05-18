using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.NuGet.Push;
using Cake.Frosting;

namespace BuildScripts;

[TaskName(nameof(DeployToGitHubTask))]
public sealed class DeployToGitHubTask : FrostingTask<BuildContext>
{
    public override bool ShouldRun(BuildContext context)
    {
        return context.IsRunningOnGitHubActions;
    }

    public override void Run(BuildContext context)
    {
        DotNetNuGetPushSettings pushSettings = new DotNetNuGetPushSettings()
        {
            Source = $"https://nuget.pkg.github.com/{context.RepositoryOwner}/index.json",
            ApiKey = context.GitHubToken
        };

        context.DotNetNuGetPush("artifacts/*.nupkg", pushSettings);
    }
}
