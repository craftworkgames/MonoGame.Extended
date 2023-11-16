using System.Linq;
using Cake.Common;
using Cake.Common.Build;
using Cake.Common.Xml;
using Cake.Core;
using Cake.Frosting;

namespace BuildScripts;

public sealed class BuildContext : FrostingContext
{
    public string ArtifactsDirectory { get; }
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
        Version = context.XmlPeek("Directory.Build.targets", "/Project/PropertyGroup/Version");
        SolutionPath = "./MonoGame.Extended.sln";
        IsRunningOnGitHubActions = context.BuildSystem().IsRunningOnGitHubActions;

        if (IsRunningOnGitHubActions)
        {
            Version += "." + context.EnvironmentVariable("GITHUB_RUN_NUMBER");
            RepositoryOwner = context.EnvironmentVariable("GITHUB_REPOSITORY_OWNER");
            RepositoryUrl = $"https://github.com/{context.EnvironmentVariable("GITHUB_REPOSITORY")}";
            IsTag = context.EnvironmentVariable("GITHUB_REF_TYPE") == "tag";
            GitHubToken = context.EnvironmentVariable("GITHUB_TOKEN");
            MyGetAccessToken = context.EnvironmentVariable("MYGET_ACCESS_TOKEN");
            NuGetAccessToken = context.EnvironmentVariable("NUGET_ACCESS_TOKEN");
        }
    }
}
