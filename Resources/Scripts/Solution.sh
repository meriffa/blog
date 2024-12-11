#!/bin/bash

# Create new solution
dotnet new sln --name ByteZoo.Blog --output ./Sources

# Create new projects
dotnet new classlib --language "C#" --output ./Sources/ByteZoo.Blog.Common
dotnet new console  --language "C#" --output ./Sources/ByteZoo.Blog.App

# Add solution projects
dotnet sln ./Sources/ByteZoo.Blog.sln add ./Sources/ByteZoo.Blog.Common/ByteZoo.Blog.Common.csproj
dotnet sln ./Sources/ByteZoo.Blog.sln add ./Sources/ByteZoo.Blog.App/ByteZoo.Blog.App.csproj

# Add package references
dotnet add ./Sources/ByteZoo.Blog.Common/ByteZoo.Blog.Common.csproj package Microsoft.Extensions.Hosting
dotnet add ./Sources/ByteZoo.Blog.App/ByteZoo.Blog.App.csproj package CommandLineParser
dotnet add ./Sources/ByteZoo.Blog.App/ByteZoo.Blog.App.csproj package Microsoft.Extensions.Hosting

# Add project references
dotnet add ./Sources/ByteZoo.Blog.App/ByteZoo.Blog.App.csproj reference ./Sources/ByteZoo.Blog.Common/ByteZoo.Blog.Common.csproj