@ECHO OFF
SET Arbor.Build.Bootstrapper.AllowPrerelease=true
SET Arbor.Build.Build.Bootstrapper.AllowPrerelease=true
SET Arbor.Build.NuGet.Package.Artifacts.Suffix=
SET Arbor.Build.NuGet.Package.Artifacts.BuildNumber.Enabled=
SET Arbor.Build.NuGetPackageVersion=
SET Arbor.Build.Vcs.Branch.Name.Version.OverrideEnabled=true
SET Arbor.Build.Vcs.Branch.Name=%GITHUB_REF%
SET Arbor.Build.VariableOverrideEnabled=true
SET Arbor.Build.Artifacts.CleanupBeforeBuildEnabled=true
SET Arbor.Build.NetAssembly.Configuration=
SET Arbor.Build.MSBuild.NuGetRestore.Enabled=true
SET Arbor.Build.Tools.External.Xunit.NetCoreApp.Enabled=false
SET Arbor.Build.BuildNumber.UnixEpochSecondsEnabled=true
SET Arbor.Build.Tools.External.MSBuild.DotNet.Enabled=true
SET Arbor.Build.Vcs.Branch.BranchModel=GitFlowBuildOnMain
SET Fallback.Version.Build=0
SET CI_BUILD=true
SET Arbor.Build.Tests.AssemblyStartsWith=Arbor.ModelBinding.Tests,Arbor.Build.Tests.Generators.Tests,Arbor.ModelBinding.Primitives.Tests

IF "%Arbor.Build.Bootstrapper.AllowPrerelease%" == "" (
	SET Arbor.Build.Bootstrapper.AllowPrerelease=true
)

SET Arbor.Build.NuGet.ReinstallArborPackageEnabled=true
SET Arbor.Build.NuGet.VersionUpdateEnabled=false
SET Arbor.Build.Artifacts.PdbArtifacts.Enabled=true
SET Arbor.Build.NuGet.Package.CreateNuGetWebPackages.Enabled=true
CALL dotnet arbor-build

EXIT /B %ERRORLEVEL%