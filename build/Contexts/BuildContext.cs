using System;
using Cake.Common.Build.GitHubActions.Data;
using Cake.Common.Xml;

namespace BuildScripts.Contexts;

public sealed class BuildContext : FrostingContext
{
    public string ArtifactsDirectory { get; }
    public string BuildConfiguration { get; }
    public string Version { get; }
    public string SolutionPath { get; }
    public string? RepositoryOwner { get; }
    public string? RepositoryUrl { get; }
    public bool IsTag { get; }
    public bool IsRunningOnGitHubActions { get; }
    public string? GitHubToken { get; }
    public string? MyGetAccessToken { get; }
    public string? NuGetAccessToken { get; }

    public BuildContext(ICakeContext context) : base(context)
    {
        ArtifactsDirectory = context.Argument(nameof(ArtifactsDirectory), "artifacts");
        BuildConfiguration = context.Argument(nameof(BuildConfiguration), "Release");
        Version = GetVersion(context);
        SolutionPath = "./MonoGame.Extended.sln";
        IsRunningOnGitHubActions = context.BuildSystem().IsRunningOnGitHubActions;

        if (IsRunningOnGitHubActions)
        {
            RepositoryOwner = context.EnvironmentVariable("GITHUB_REPOSITORY_OWNER");
            RepositoryUrl = $"https://github.com/{context.EnvironmentVariable("GITHUB_REPOSITORY")}";
            IsTag = context.EnvironmentVariable("GITHUB_REF_TYPE") == "tag";
            GitHubToken = context.EnvironmentVariable("GITHUB_TOKEN");
            MyGetAccessToken = context.EnvironmentVariable("MYGET_ACCESS_TOKEN");
            NuGetAccessToken = context.EnvironmentVariable("NUGET_ACCESS_TOKEN");
        }
    }


    // -------------------------------------------------------------------------
    // Gets the version to use during builds of MonoGame extended.
    // Version value that is generated uses SemVersion with the following format
    //
    // [Major].[Minor].[Path].[Build](-develop | owner)
    //
    // e.g. 1.2.3.4 or 1.2.3.4-develop or 1.2.3.4-AristurtleDev
    // -------------------------------------------------------------------------
    private static string GetVersion(ICakeContext context)
    {
        //  Read version from the Directory.Build.targets file
        var version = context.XmlPeek("Directory.Build.targets", "/Project/PropertyGroup/Version");

        //  If a version was supplied as an argument, it supersedes the version in the targets file
        if(context.HasArgument(nameof(Version)))
        {
            version = context.Argument(nameof(Version), version);
        }

        version = $"{version}.1-develop";

        if(context.BuildSystem().IsRunningOnGitHubActions)
        {
            var workflow = context.BuildSystem().GitHubActions.Environment.Workflow;
            version = $"{version}.{workflow.RunNumber}";

            if(workflow.Repository != "craftworkgames/MonoGame.Extended")
            {
                //  Not running from official MonoGame.Extended repository, attach repository owner to version
                version += $"-{workflow.RepositoryOwner}";
            }
            else if(workflow.RefType == GitHubActionsRefType.Branch && workflow.RefName == "refs/heads/develop")
            {
                //  This was either a push to the develop branch or a nightly build, attach -develop to the version.
                version = $"-develop";
            }
        }

        return version;
    }
}
