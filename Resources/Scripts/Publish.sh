#!/bin/bash

# Self-Contained Application
dotnet publish ./Sources/ByteZoo.Blog.App/ByteZoo.Blog.App.csproj --output ./Sources/ByteZoo.Blog.App/bin/Publish --configuration Release --runtime linux-x64 --self-contained
# Self-Contained Application (ReadyToRun)
dotnet publish ./Sources/ByteZoo.Blog.App/ByteZoo.Blog.App.csproj --output ./Sources/ByteZoo.Blog.App/bin/Publish --configuration Release --runtime linux-x64 --self-contained -p:PublishReadyToRun=true
# Self-Contained Application (Single File)
dotnet publish ./Sources/ByteZoo.Blog.App/ByteZoo.Blog.App.csproj --output ./Sources/ByteZoo.Blog.App/bin/Publish --configuration Release --runtime linux-x64 --self-contained -p:PublishSingleFile=true --p:DebugType=embedded -p:IncludeNativeLibrariesForSelfExtract=true
# Self-Contained Application (Single File, ReadyToRun)
dotnet publish ./Sources/ByteZoo.Blog.App/ByteZoo.Blog.App.csproj --output ./Sources/ByteZoo.Blog.App/bin/Publish --configuration Release --runtime linux-x64 --self-contained -p:PublishReadyToRun=true -p:PublishSingleFile=true --p:DebugType=embedded -p:IncludeNativeLibrariesForSelfExtract=true

# Native AOT Application
dotnet publish ./Sources/ByteZoo.Blog.App/ByteZoo.Blog.App.csproj --output ./Sources/ByteZoo.Blog.App/bin/Publish --configuration Release --use-current-runtime -p:PublishAot=true