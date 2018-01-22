#r "packages/FAKE/tools/FakeLib.dll" 
open Fake
open Fake.AssemblyInfoFile
open System.Text
open System

// Parameters
let outputDir = getBuildParamOrDefault "OutputDir" "C:\inetpub\wwwroot\GrandfathersApiary\Website"
let buildConfiguration = getBuildParamOrDefault "BuildConfiguration" "Debug"
let installWebpack = getBuildParamOrDefault "InstallWebpack" "false"
let unicornItems = outputDir + "\data\unicorn\src"
let version = getBuildParamOrDefault "Version" "1.0.0.0"
let appReferences  =
    !! "src/feature/**/code/*.csproj"
      ++ "src/foundation/**/code/*.csproj"
        ++ "src/project/**/code/*.csproj"

let assemblyFiles  =
    !! "src/feature/**/code/Properties/AssemblyInfo.cs"
      ++ "src/foundation/**/code/Properties/AssemblyInfo.cs"
        ++ "src/project/**/code/Properties/AssemblyInfo.cs"

// Targets
Target "Show build information" (fun _ -> 
    log ("Output directory: " + outputDir) 
    log ("Build configuration: " + buildConfiguration) 
    log ("Install webpack condition: " + installWebpack) 
    log ("Version: " + version) 
)

Target "Restore packages" (fun _ -> 
    RestorePackages()
)

Target "Reset unicorn configuration" (fun _ ->
    let unicornConfigs  =
        !! (outputDir + "\App_Config\Include\Feature\*.Serialization.config")
            ++ (outputDir + "\App_Config\Include\Foundation\*.Serialization.config")
                ++ (outputDir + "\App_Config\Include\GrandfathersApiary.*.Serialization.config")
                
    DeleteFiles unicornConfigs
)

Target "Generate assembly info"(fun _ ->
    for af in assemblyFiles do 
            UpdateAttributes  af
                [Attribute.Product "Sitecore helix learning site"
                 Attribute.Copyright "Copyright Marenych Igor 2017"
                 Attribute.Company "marenych Igor"
                 Attribute.Configuration buildConfiguration
                 Attribute.Version version
                 Attribute.FileVersion version]
)
  
Target "Publish website" (fun _ ->
    RegexReplaceInFileWithEncoding "<PublishUrl>(.*?)<\/PublishUrl>" ("<PublishUrl>" + outputDir + "</PublishUrl>") Encoding.UTF8 "main.pubxml"
    appReferences
    |> MSBuild "" "Build" ["Configuration",buildConfiguration; "DeployOnBuild", "true"; "PublishProfile", "local.pubxml"]
    |> Log "AppBuild-Output: "
)

Target "Clean unicorn items directory" (fun _ ->
    CleanDir unicornItems
)

Target "Copy unicorn items" (fun _ ->
    CopyDir unicornItems "src" (fun x -> x.EndsWith(".yml"))
)

// Workflow
"Show build information"
    ==> "Restore packages"
    ==> "Reset unicorn configuration"
    =?> ("Generate assembly info", version <> "1.0.0.0")
    ==> "Publish website"
    ==> "Clean unicorn items directory"
    ==> "Copy unicorn items"
    
RunTargetOrDefault "Copy unicorn items"