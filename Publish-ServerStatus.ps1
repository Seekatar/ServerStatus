param(
[Parameter(Mandatory)]
[ValidateSet("win10-x64","ubuntu.16.04-x64","win8-arm","ubuntu.16.04-arm")]
[string] $Runtime,
[ValidateScript({Test-Path $_ -PathType Container})]
[string] $Folder
)

dotnet publish -r $Runtime -o $Folder