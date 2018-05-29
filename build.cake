#tool nuget:?package=vswhere
#tool nuget:?package=NUnit.Runners&version=2.6.4
#tool nuget:?package=GitVersion.CommandLine

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var solution = "./Source/MonoGame.Extended.sln";

var vsLatest  = VSWhereLatest();
var msBuildPath = vsLatest?.CombineWithFilePath("./MSBuild/15.0/Bin/amd64/MSBuild.exe");

Task("Restore")
    .Does(() =>
{
    Information("##teamcity[progressMessage 'Restoring packages...']");    
    DotNetCoreRestore(solution);
});

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
{
    Information("##teamcity[progressMessage 'Building solution...']");    
    DotNetCoreBuild(solution, new DotNetCoreBuildSettings 
    {
        Configuration = configuration
    });
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
    Information("##teamcity[progressMessage 'Running tests...']");    
    var testRuns = 0;
    var failedRuns = 0;

    foreach (var project in GetFiles($"./Source/Tests/**/*.Tests.csproj"))
    { 
        try
        {
            // var filename = project.GetFilename().ChangeExtension("dll");
            // var testDll = project.GetDirectory().CombineWithFilePath($"bin/{configuration}/netcoreapp2.0/{filename}");
            Information("Test Run {0} - {1}", testRuns++, project);
            DotNetCoreTest(project.FullPath);            
        }
        catch
        {
            failedRuns++;
        }
    }

    if(failedRuns > 0)
        throw new Exception($"{failedRuns} of {testRuns} test runs failed.");
});

Task("Pack")
    .IsDependentOn("Test")
    .Does(() =>
{
    Information("##teamcity[progressMessage 'Packing packages...']");    
    var artifactsDirectory = "./artifacts";
    var gitVersion = GitVersion();
    CreateDirectory(artifactsDirectory);
    CleanDirectory(artifactsDirectory);    

    foreach (var project in GetFiles($"./Source/MonoGame.Extended*/*.csproj"))
    {
        DotNetCorePack(project.FullPath, new DotNetCorePackSettings 
        {
            IncludeSymbols = true,
            OutputDirectory = artifactsDirectory,
            ArgumentCustomization = args => args.Append($"/p:Version={gitVersion.NuGetVersion}")
        });
    }
});

Task("Default")
    .IsDependentOn("Pack");

RunTarget(target);
