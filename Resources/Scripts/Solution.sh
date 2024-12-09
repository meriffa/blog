#!/bin/bash

# Create new solution
dotnet new sln --name ByteZoo.Blog --output ./Sources

# Create new projects
dotnet new console  --language "C#" --output ./Sources/ByteZoo.Blog.Console

# Add solution projects
dotnet sln ./Sources/ByteZoo.Blog.sln add ./Sources/ByteZoo.Blog.Console/ByteZoo.Blog.Console.csproj