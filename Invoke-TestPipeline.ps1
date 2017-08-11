<#
.SYNOPSIS
Helper to run test pipelines

.PARAMETER Type
type of test to run

.PARAMETER PiName
name of pipeline to show in UI

.PARAMETER delay
delay parameter passed to test pipeline

.EXAMPLE
Invoke-TestPipeline -Type Ok -PiName Ok03

#>
function Invoke-TestPipeline
{
param(
[ValidateSet("Ok","Fail","Pause")]
[string] $Type,
[string] $PiName = "Test",
[int] $delay = 1
)

switch ( $Type )
{
    "Fail" { $name = "Ten Steps to Failure" }
    "Pause" {$name = "Ten Steps to Intervention"}
    "Ok" { $name = "Ten Steps to Success"}

}

$project = "PesterTestProject"
$baseUri = "http://hackweek:8080"
$CtmToken = "5988590a683f2444015568fc"
$desc = "Test for $Type"


Invoke-RestMethod -Uri "$baseUri/api/initiate_pipeline?definition=$name&project=$project&group=Test&details={`"stepDelay`":  $delay,`"description`":`"$desc`",`"name`":`"$piName`"}" `
            -Headers  @{Accept="application/json";Authorization="token $CtmToken"} -Verbose

}





