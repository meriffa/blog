#!/bin/bash

# Display .NET Performance Counters
dotnet-counters ps                                                                              # List .NET processes
dotnet-counters monitor -p <PID> --refresh-interval 1 --counters System.Runtime                 # Monitor System.Runtime counters