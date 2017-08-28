param(
[Parameter(Mandatory)]
[ValidateSet("win-x64","linux-x64","win8-arm","linux-arm")]
[string] $Runtime,
[ValidateScript({Test-Path $_ -PathType Container})]
[string] $Folder
)

dotnet publish -r $Runtime -o $Folder