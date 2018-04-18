#tool nuget:?package=vswhere
#tool nuget:?package=NUnit.Runners&version=2.6.4

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var solution = "./Source/MonoGame.Extended.sln";

var vsLatest  = VSWhereLatest();
var msBuildPath = vsLatest?.CombineWithFilePath("./MSBuild/15.0/Bin/amd64/MSBuild.exe");

Task("Restore")
    .Does(() =>
{
    NuGetRestore(solution);
});

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
{
    Information("Build using {0}", msBuildPath);
    MSBuild(solution, new MSBuildSettings 
    {
        Verbosity = Verbosity.Minimal,
        ToolVersion = MSBuildToolVersion.VS2017,
        ToolPath = msBuildPath,
        Configuration = configuration
    });
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
    // TODO: Get the tests working!
    // NUnit($"./Source/Tests/**/bin/{configuration}/*.Tests.dll", new NUnitSettings 
    // {
    //     ShadowCopy = false,
    //     X86 = true
    // });
});

Task("Default")
    .IsDependentOn("Test");

RunTarget(target);
