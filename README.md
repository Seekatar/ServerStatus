# Server Status Page and API
This is a web API and React app that gets build statuses from Continuum and alerts from Zabbix.  This is a port of an Arduino project that displays the statuses with LEDs.  Now that project calls this one's API to avoid the Arduino having to know credentials.  This project was built with Visual Studio Code using ASP.NET Core 2.0 (Preview).  I initially created an project using the `webapi` template using a .NET Core Razor Page for the UI.  Then I created one using the `react` template for the UI and merged in the API code.

## Setup
To use, download [.NET Core 2.0 Preview](https://www.microsoft.com/net/core/preview#windowscmd).  VS Code and VStudio 2017 Preview have good support for Core 2.

After getting the repo, add a file `appsettings.Development.json` and set your Continuum key and the Admin Zabbix password.  DO NOT COMMIT THIS FILE!.  (It's in the git ignore.)
```Json
{
  "Config" : {
      "CONTINUUM_KEY" :"zzzzzzzzzzzzzzzzz",
      "ZABBIX_PW" : "zzzzzzzzzzzzzzzzz"
  }
}

```
## Build and Run
You can debug from Visual Studio Code or Visual Studio 2017 (15.3.0 Preview 2.0).  In VSCode use the `.NET Core Launch (web)` debug configuration to make sure that the environment gets set up correctly and webpack is launched.

To run manually, in the repo's folder:

```PowerShell
dotnet restore
dotnet run
```
And then hit the endpoint http://localhost:5000/api/status for JSON or http://localhost:5000 for the react page.

I've tested this on Windows 10 and Ubuntu 16.04.  The csproj has those two runtimes configured in the `RuntimeIdentifiers` element.

## Publishing To Ubuntu
```PowerShell
# on the dev box publish it
dotnet publish --runtime "ubuntu.16.04-x64" `
  --output c:\temp\ServerStatusUbuntu `
  --configuration Release
```
Copy the output to Ubuntu box.  **!! Make sure dotnet core is same version on target, and there are no extra files on target (DLLs, etc)** For my environment, I created a `dotnet` user to copy and run the app.

Add a file appsettings.Production.json and set your Continuum key and the Admin Zabbix password as above

### To run it on Ubuntu:

```PowerShell
dotnet V1ServerStatus.dll
```
### Setting up as a service on Ubuntu
https://docs.microsoft.com/en-us/aspnet/core/publishing/linuxproduction
