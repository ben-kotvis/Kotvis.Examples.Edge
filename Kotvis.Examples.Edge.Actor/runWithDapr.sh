#!/bin/bash

dotnet Kotvis.Examples.Edge.Actor.dll & 
sleep 3
daprd --app-id kotvisexamplesedgeactor --app-port 3000 --placement-host-address placement:50005 --dapr-grpc-port 50001 --components-path components --log-level debug