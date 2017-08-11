set-CtmdefaultConfig local -Verbose

$pi = "598b312a683f244867bf1321"
$step = 5
$phase = "Phase the first"
$stage = "Stage the first"

$parameters = @()
$parameters += "pi=$pi"
$parameters += "phase=$phase"
$parameters += "stage=$stage"
$parameters += "step=$step"
$parameters += "status=failure"
$parameters += "result_key=result_key"
$parameters += "detail=I did it!"

$x = Invoke-WebRequest -Uri "http://hackweek:8080/api/update_pi_step_status?$($parameters -join "&")" -Headers @{Authorization="token 5988590a683f2444015568fc";Accept="application/json"} 
$x.content
#Invoke-CtmApi -Command "update_pi_step_status" -Parameters  -Verbose

