using Cake.Common.Tools.DotNet.Test;

namespace BuildScripts;

[TaskName(nameof(TestTask))]
public class TestTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        DotNetTestSettings testSettings = new DotNetTestSettings()
        {
            Configuration = "Release",
            Verbosity = DotNetVerbosity.Normal,
            NoLogo = true,
            NoBuild = true
        };

        context.DotNetTest(context.SolutionPath, testSettings);
    }
}
