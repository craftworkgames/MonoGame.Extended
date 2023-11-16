using System.Threading.Tasks;
using Cake.Common.Build;
using Cake.Common.IO;
using Cake.Common.Tools.ReportUnit;
using Cake.Core;
using Cake.Core.IO;
using Cake.Frosting;

namespace BuildScripts;

public static class Program
{
    public static int Main(string[] args)
    {
        return new CakeHost()
            .UseContext<BuildContext>()
            .UseWorkingDirectory("../")
            .Run(args);
    }
}

[TaskName("Default")]
[IsDependentOn(typeof(RestoreTask))]
[IsDependentOn(typeof(BuildTask))]
[IsDependentOn(typeof(TestTask))]
[IsDependentOn(typeof(PackageTask))]
public sealed class DefaultTask : FrostingTask {}


[TaskName("Deploy")]
[IsDependentOn(typeof(DeployToGitHubTask))]
[IsDependentOn(typeof(DeployToMyGetTask))]
[IsDependentOn(typeof(DeployToNuGetTask))]
public sealed class DeployTask : FrostingTask {}
