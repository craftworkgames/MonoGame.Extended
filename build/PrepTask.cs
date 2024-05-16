namespace BuildScripts;

[TaskName("Prepare")]
public sealed class PrepTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.CleanDirectory(context.ArtifactsDirectory);
        context.CreateDirectory(context.ArtifactsDirectory);
    }
}
