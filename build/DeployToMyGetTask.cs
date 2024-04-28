using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.NuGet.Push;
using Cake.Frosting;

namespace BuildScripts;

[TaskName(nameof(DeployToMyGetTask))]
public sealed class DeployToMyGetTask : FrostingTask<BuildContext>
{
    public override bool ShouldRun(BuildContext context)
    {
        return context.IsRunningOnGitHubActions;
    }

    public override void Run(BuildContext context)
    {
        DotNetNuGetPushSettings pushSettings = new DotNetNuGetPushSettings()
        {
            Source = $"https://www.myget.org/F/lithiumtoast/api/v3/index.json",
            ApiKey = context.MyGetAccessToken
        };

        context.DotNetNuGetPush("artifacts/*.nupkg", pushSettings);
    }
}
