

# http://172.16.60.44:8080/flow/pi_detail?id=598b3168683f244867bf1331
    Invoke-RestMethod 'http://hackweek:5000/api/Status/pipelineInstance/598b6990683f244867bf134d/Phase%201/Stage%201/6' `
    -Method Post -Body (ConvertTo-Json @{response="response to it";outputKey="key";confirm="True"}) -ContentType 'application/json'


