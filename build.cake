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
    var testRuns = 0;
    var failedRuns = 0;

    foreach (var project in GetFiles($"./Source/Tests/**/*.Tests.csproj"))
    { 
        try
        {
            var filename = project.GetFilename().ChangeExtension("dll");
            var testDll = project.GetDirectory().CombineWithFilePath($"bin/{configuration}/{filename}");
            Information("Test Run {0} - {1}", testRuns++, filename);
            NUnit(testDll.FullPath, new NUnitSettings 
            {
                ShadowCopy = false
            });            
        }
        catch
        {
            failedRuns++;
        }
    }

    if(failedRuns > 0)
        throw new Exception($"{failedRuns} of {testRuns} test runs failed.");
});

Task("Default")
    .IsDependentOn("Test");

RunTarget(target);
