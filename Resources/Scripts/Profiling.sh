#!/bin/bash

# Setup Profiler
export CORECLR_ENABLE_PROFILING=1
export CORECLR_PROFILER={ECB669ED-DDD3-4BCD-85C8-A023EC310FE2}
export CORECLR_PROFILER_PATH=~/Development/ByteZoo.Blog/Sources/ByteZoo.Blog.Profiler/bin/Publish/ByteZoo.Blog.Profiler.so

# Configure Profiler
export BYTEZOO_BLOG_PROFILER_MONITOR_THREADS=1
export BYTEZOO_BLOG_PROFILER_MONITOR_MODULES=1
export CONFIG_MONITOR_ALLOCATED_TYPE_NAME=ByteZoo.Blog.App.Controllers.Concepts.StringController
export BYTEZOO_BLOG_PROFILER_ENABLE_STACK_SNAPSHOT=1