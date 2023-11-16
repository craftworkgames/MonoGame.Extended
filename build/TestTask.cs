
using Cake.Common.IO;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Test;
using Cake.Core.IO;
using Cake.Frosting;

namespace BuildScripts;

[TaskName(nameof(TestTask))]
public class TestTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        DotNetTestSettings settings = new DotNetTestSettings()
        {
            Configuration = "Release",
        };

        context.DotNetTest("MonoGame.Extended.sln");
    }
}
