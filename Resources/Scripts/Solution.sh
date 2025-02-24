#!/bin/bash

# Create new solution
dotnet new sln --name ByteZoo.Blog --output ./Sources

# Create new projects
dotnet new classlib --language "C#" --output ./Sources/ByteZoo.Blog.Common
dotnet new console  --language "C#" --output ./Sources/ByteZoo.Blog.App
dotnet new webapp   --language "C#" --output ./Sources/ByteZoo.Blog.Web

# Add solution projects
dotnet sln ./Sources/ByteZoo.Blog.sln add ./Sources/ByteZoo.Blog.Common/ByteZoo.Blog.Common.csproj
dotnet sln ./Sources/ByteZoo.Blog.sln add ./Sources/ByteZoo.Blog.App/ByteZoo.Blog.App.csproj
dotnet sln ./Sources/ByteZoo.Blog.sln add ./Sources/ByteZoo.Blog.Web/ByteZoo.Blog.Web.csproj

# Add package references
dotnet add ./Sources/ByteZoo.Blog.Common/ByteZoo.Blog.Common.csproj package Microsoft.EntityFrameworkCore.Design
dotnet add ./Sources/ByteZoo.Blog.Common/ByteZoo.Blog.Common.csproj package Microsoft.EntityFrameworkCore.Sqlite
dotnet add ./Sources/ByteZoo.Blog.Common/ByteZoo.Blog.Common.csproj package Microsoft.EntityFrameworkCore.SqlServer
dotnet add ./Sources/ByteZoo.Blog.Common/ByteZoo.Blog.Common.csproj package Microsoft.Extensions.Hosting
dotnet add ./Sources/ByteZoo.Blog.Common/ByteZoo.Blog.Common.csproj package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add ./Sources/ByteZoo.Blog.Common/ByteZoo.Blog.Common.csproj package Pomelo.EntityFrameworkCore.MySql --prerelease
dotnet add ./Sources/ByteZoo.Blog.App/ByteZoo.Blog.App.csproj package BenchmarkDotNet
dotnet add ./Sources/ByteZoo.Blog.App/ByteZoo.Blog.App.csproj package CommandLineParser
dotnet add ./Sources/ByteZoo.Blog.App/ByteZoo.Blog.App.csproj package Microsoft.Diagnostics.NETCore.Client
dotnet add ./Sources/ByteZoo.Blog.App/ByteZoo.Blog.App.csproj package Microsoft.Extensions.Hosting
dotnet add ./Sources/ByteZoo.Blog.Web/ByteZoo.Blog.Web.csproj package Swashbuckle.AspNetCore

# Add project references
dotnet add ./Sources/ByteZoo.Blog.App/ByteZoo.Blog.App.csproj reference ./Sources/ByteZoo.Blog.Common/ByteZoo.Blog.Common.csproj
dotnet add ./Sources/ByteZoo.Blog.Web/ByteZoo.Blog.Web.csproj reference ./Sources/ByteZoo.Blog.Common/ByteZoo.Blog.Common.csproj