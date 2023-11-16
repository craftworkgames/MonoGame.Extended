using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.Diagnostics;
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
[IsDependentOn(typeof(BuildTask))]
[IsDependentOn(typeof(TestTask))]
[IsDependentOn(typeof(PackageTask))]
public sealed class DefaultTask : FrostingTask {}
