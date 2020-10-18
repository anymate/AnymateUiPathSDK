rm *.nupkg
.\nuget.exe pack .\Anymate.UiPath.API\Anymate.UiPath.API.csproj -IncludeReferencedProjects 
.\nuget.exe pack .\Anymate.UiPath.Auth\Anymate.UiPath.Auth -IncludeReferencedProjects 
.\nuget.exe pack .\Anymate.UiPath.Helpers\Anymate.UiPath.Helpers.csproj -IncludeReferencedProjects 

