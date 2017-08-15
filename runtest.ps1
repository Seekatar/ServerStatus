# run some tests pipelines

. (Join-Path $PSScriptRoot Invoke-TestPipeline.ps1)
. .\Invoke-TestPipeline.ps1

# test start 12 green 
foreach ($i in (1..12))
{
	Invoke-TestPipeline -Type Ok -PiName "SomethinGud$i"
}

<#
Intro: 
    * Server Sidekick team intro Denise, Holly, Andrew, Kevin & sister, me
    * Started w/Arduino status light that polled continuum and zabbix (server monitoring) for status
    * SS is a massive extension of status light with an enhance web UI and new mobile app
    * Behind all of this .NET Core ASP.NET Web API server running on Linux.  Being 
    Core, it runs on devlopers' machines running Windows or OSX
    * Lights, Cube 
    * To demostrate all this functionality, we'll run through three scenarios for you, starting with Kevin 
    as a member of QA 
#>

<# scenario 1 Kevin QA #>
Invoke-TestPipeline -Type Ok -PiName "S-1234 InlineEdit some feature" -Delay 4
<# wait for filtering.... #>
Invoke-TestPipeline -Type Ok -PiName "S-5678 Something else"
Invoke-TestPipeline -Type Fail -PiName "S-6455 Cool stuff" -Desc "Rino's important build" 
Invoke-TestPipeline -Type Ok -PiName "S-6034 You need this"

<# scenario 2 Andrew DevOps #>
<# wait for muting of failed one above #>
Invoke-TestPipeline -Type Fail -PiName "S-8462 More bugs"
Invoke-TestPipeline -Type Ok -PiName "S-8732 BADF00D"

<# scenario 3 Denise Release Mgr #>
Invoke-TestPipeline -Type Pause -PiName "S-911 Critical Hotfix" 










<# devops snooze scenario
    throw in a failure, good one another failure

	Dev ops sees failure
	Opens details
	Jumps to ctm, sees its ok to ignore
	Mutes the failure, status goes green
	Another failure comes in, goes back to red
#>
Invoke-TestPipeline -Type Fail -PiName Fail01 -SleepTime 5 -StepDelay 2 -Desc "Rinos important test"
Invoke-TestPipeline -Type Ok -PiName Ok03  -SleepTime 60 -StepDelay 3
Invoke-TestPipeline -Type Fail -PiName Fail02 -SleepTime 10
Invoke-TestPipeline -Type Ok -PiName Ok04  -SleepTime 5

<# pause scenario

	user? filters on job she's interested in
	sees it's in paused state
	opens details
	confirms the details
	(we see it change to running then success)
#>
Invoke-TestPipeline -Type Pause -PiName Pause01 -SleepTime 5
Invoke-TestPipeline -Type Ok -PiName Ok05

<# watch build scenario

	QA does merge, that kicks off a pipeline
	QA filters on that build (now all status colors are only that build)
	Once it goes green (or red), they can check it deployed ok
#>
Invoke-TestPipeline -Type Ok -PiName WatchMe -SleepTime 2 -StepDelay 5
Invoke-TestPipeline -Type Ok -PiName IgnoreMe 
Invoke-TestPipeline -Type Fail -PiName IgnoreMeToo 
Invoke-TestPipeline -Type Ok -PiName IgnoreMeTooToo 

