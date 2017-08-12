set-CtmdefaultConfig local -Verbose

$pi = "598e108d683f244867bf1487"
$step = 4
$phase = "1"
$stage = "1"

$parameters = @()
$parameters += "pi=$pi"
$parameters += "phase=$phase"
$parameters += "stage=$stage"
$parameters += "step=$step"
$parameters += "status=failure"
$parameters += "result_key=result_key"
$parameters += "detail=I did it!"

$x = Invoke-WebRequest -Uri "http://hackweek:8080/api/update_pi_step_status?$($parameters -join "&")" -Headers @{Authorization="token 5988590a683f2444015568fc";Accept="application/json"}  -Verbose
$x.content
#Invoke-CtmApi -Command "update_pi_step_status" -Parameters  -Verbose

