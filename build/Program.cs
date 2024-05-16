using BuildScripts.Contexts;

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
[IsDependentOn(typeof(PrepTask))]
[IsDependentOn(typeof(BuildTask))]
[IsDependentOn(typeof(TestTask))]
[IsDependentOn(typeof(PackageTask))]
public sealed class DefaultTask : FrostingTask {}


[TaskName("Deploy")]
[IsDependentOn(typeof(DeployToGitHubTask))]
[IsDependentOn(typeof(DeployToMyGetTask))]
[IsDependentOn(typeof(DeployToNuGetTask))]
public sealed class DeployTask : FrostingTask {}
