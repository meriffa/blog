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
dotnet add ./Sources/ByteZoo.Blog.Common/ByteZoo.Blog.Common.csproj package Microsoft.EntityFrameworkCore.Design
dotnet add ./Sources/ByteZoo.Blog.Common/ByteZoo.Blog.Common.csproj package Microsoft.EntityFrameworkCore.Sqlite
dotnet add ./Sources/ByteZoo.Blog.Common/ByteZoo.Blog.Common.csproj package Microsoft.EntityFrameworkCore.SqlServer
dotnet add ./Sources/ByteZoo.Blog.Common/ByteZoo.Blog.Common.csproj package Microsoft.Extensions.Hosting
dotnet add ./Sources/ByteZoo.Blog.Common/ByteZoo.Blog.Common.csproj package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add ./Sources/ByteZoo.Blog.Common/ByteZoo.Blog.Common.csproj package Pomelo.EntityFrameworkCore.MySql --prerelease
dotnet add ./Sources/ByteZoo.Blog.App/ByteZoo.Blog.App.csproj package BenchmarkDotNet
dotnet add ./Sources/ByteZoo.Blog.App/ByteZoo.Blog.App.csproj package CommandLineParser
dotnet add ./Sources/ByteZoo.Blog.App/ByteZoo.Blog.App.csproj package Microsoft.Extensions.Hosting

# Add project references
dotnet add ./Sources/ByteZoo.Blog.App/ByteZoo.Blog.App.csproj reference ./Sources/ByteZoo.Blog.Common/ByteZoo.Blog.Common.csproj

# Database setup
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialVersion -p ./Sources/ByteZoo.Blog.Common -o ./EntityFramework/Migrations -- SQLite "Data Source=<DatabaseFolder>/ByteZoo.Blog.Data.db"
dotnet ef database update -p ./Sources/ByteZoo.Blog.Common -- SQLite "Data Source=<DatabaseFolder>/ByteZoo.Blog.Data.db"
dotnet ef migrations add InitialVersion -p ./Sources/ByteZoo.Blog.Common -o ./EntityFramework/Migrations -- PostgreSQL "Host=<Host>;Username=<User>;Password=<Password>;Database=ByteZoo.Blog.Data;"
dotnet ef database update -p ./Sources/ByteZoo.Blog.Common -- PostgreSQL "Host=<Host>;Username=<User>;Password=<Password>;Database=ByteZoo.Blog.Data;"
dotnet ef migrations add InitialVersion -p ./Sources/ByteZoo.Blog.Common -o ./EntityFramework/Migrations -- MySQL "server=<Host>;user=<User>;password=<Password>;database=ByteZoo.Blog.Data;"
dotnet ef database update -p ./Sources/ByteZoo.Blog.Common -- MySQL "server=<Host>;user=<User>;password=<Password>;database=ByteZoo.Blog.Data;"
dotnet ef migrations add InitialVersion -p ./Sources/ByteZoo.Blog.Common -o ./EntityFramework/Migrations -- SQLServer "Data Source=<Host>;User ID=<User>;Password=<Password>;Initial Catalog=ByteZoo.Blog.Data;Encrypt=false;"
dotnet ef database update -p ./Sources/ByteZoo.Blog.Common -- SQLServer "Data Source=<Host>;User ID=<User>;Password=<Password>;Initial Catalog=ByteZoo.Blog.Data;Encrypt=false;"

# Benchmarks
cd ./Sources/ByteZoo.Blog.App && dotnet run -c Release -- Tools-Benchmark -a "\"--filter *IntrinsicsController*\""