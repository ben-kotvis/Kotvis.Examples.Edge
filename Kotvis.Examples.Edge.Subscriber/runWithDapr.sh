#!/bin/bash

dotnet Kotvis.Examples.Edge.Subscriber.dll &
sleep 3
daprd --app-id subscriber --placement-host-address placement:50005 --components-path components --log-level debug