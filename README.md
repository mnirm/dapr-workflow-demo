# Workflow demo
## Run dapr
```dapr run --app-id wfapp --dapr-grpc-port 4001 --dapr-http-port 3500```

> Make sure your dapr cli version and NugetPackage version match.

## Start new instance on workflow

- `curl -i -X POST http://localhost:3500/v1.0-alpha1/workflows/dapr/TestWorkflow/start?instanceID=12345678 \
  -H "Content-Type: application/json" \
  -d '{"input": "test input not working"}'`


<br />
<br />
<br />

__When upgrading to 1.11 dapr cli and nuget package this happens__

```
INFO[0397] 12345678: starting new 'TestWorkflow' instance with ID = '12345678'.  app_id=wfapp instance=-MacBook-Pro.local scope=dapr.runtime.wfengine type=log ver=1.11.0  
INFO[0398] 12345678: 'TestWorkflow' completed with a FAILED status.  app_id=wfapp instance=-MacBook-Pro.local scope=dapr.runtime.wfengine type=log ver=1.11.0
```

- `curl -i -X GET http://localhost:3500/v1.0-alpha1/workflows/dapr/12345678`

```
HTTP/1.1 200 OK
Date: Wed, 21 Jun 2023 06:55:10 GMT
Content-Type: application/json
Content-Length: 493
Traceparent: 00-f5f0d9364590aa509aa3780d37db80d0-6eb10bf9a05c9c5e-01

{"instanceID":"12345678","workflowName":"TestWorkflow","createdAt":"2023-06-21T06:54:57.444920Z","lastUpdatedAt":"2023-06-21T06:54:57.546537Z","runtimeStatus":"FAILED","properties":{"dapr.workflow.custom_status":"","dapr.workflow.failure.error_message":"The JSON value could not be converted to System.String. Path: $ | LineNumber: 0 | BytePositionInLine: 1.","dapr.workflow.failure.error_type":"System.Text.Json.JsonException","dapr.workflow.input":"{\"input\": \"test input not working\"}"}}%
```