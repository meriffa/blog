#!/bin/bash

# Run Benchmarks
cd ./Sources/ByteZoo.Blog.App && dotnet run -c Release -- Tools-Benchmark -a "\"--filter *IntrinsicsController*\""
cd ./Sources/ByteZoo.Blog.App && dotnet run -c Release -- Tools-Benchmark -a "\"--filter *MemoryAllocationController*\""