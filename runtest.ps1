# run some tests pipelines
. (Join-Path $PSScriptRoot Invoke-TestPipeline.ps1)

Invoke-TestPipeline -Type Ok -PiName Ok03
Start-Sleep -Seconds 5
Invoke-TestPipeline -Type Pause -PiName Pause01
Start-Sleep -Seconds 5
Invoke-TestPipeline -Type Fail -PiName Fail01