using Cake.Common.Tools.DotNet.Restore;

namespace BuildScripts;

public sealed class RestoreTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        DotNetRestoreSettings restoreSettings = new DotNetRestoreSettings()
        {
            Verbosity = DotNetVerbosity.Quiet
        };

        context.DotNetRestore(context.SolutionPath, restoreSettings);
    }
}
