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
    Information("##teamcity[blockOpened name='Restore']");
    Information("##teamcity[progressMessage 'Restoring packages...']");    
    DotNetCoreRestore(solution);
    Information("##teamcity[blockClosed name='Restore']");
});

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
{
    Information("##teamcity[blockOpened name='Build']");
    Information("##teamcity[progressMessage 'Building solution...']");    

    var buildSettings = new DotNetCoreBuildSettings { Configuration = configuration };

    // first we build the Extended Content Pipeline DLL as a workaround to issue #495
    DotNetCoreBuild($"./Source/MonoGame.Extended.Content.Pipeline/MonoGame.Extended.Content.Pipeline.csproj", buildSettings);
        
    // then we can build the rest of the solution
    DotNetCoreBuild(solution, buildSettings);
    
    Information("##teamcity[blockClosed name='Build']");
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
    Information("##teamcity[blockOpened name='Test']");
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

    Information("##teamcity[blockClosed name='Test']");
});

Task("Pack")
    .IsDependentOn("Test")
    .Does(() =>
{
    Information("##teamcity[blockOpened name='Pack']");
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

    Information("##teamcity[blockClosed name='Pack']");
});

Task("Default")
    .IsDependentOn("Pack");

RunTarget(target);
