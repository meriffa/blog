#!/bin/bash

# Setup Databases
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialVersion -p ./Sources/ByteZoo.Blog.Common -o ./EntityFramework/Migrations -- SQLite "Data Source=<DatabaseFolder>/ByteZoo.Blog.Data.db"
dotnet ef database update -p ./Sources/ByteZoo.Blog.Common -- SQLite "Data Source=<DatabaseFolder>/ByteZoo.Blog.Data.db"
dotnet ef migrations add InitialVersion -p ./Sources/ByteZoo.Blog.Common -o ./EntityFramework/Migrations -- PostgreSQL "Host=<Host>;Username=<User>;Password=<Password>;Database=ByteZoo.Blog.Data;"
dotnet ef database update -p ./Sources/ByteZoo.Blog.Common -- PostgreSQL "Host=<Host>;Username=<User>;Password=<Password>;Database=ByteZoo.Blog.Data;"
dotnet ef migrations add InitialVersion -p ./Sources/ByteZoo.Blog.Common -o ./EntityFramework/Migrations -- MySQL "server=<Host>;user=<User>;password=<Password>;database=ByteZoo.Blog.Data;"
dotnet ef database update -p ./Sources/ByteZoo.Blog.Common -- MySQL "server=<Host>;user=<User>;password=<Password>;database=ByteZoo.Blog.Data;"
dotnet ef migrations add InitialVersion -p ./Sources/ByteZoo.Blog.Common -o ./EntityFramework/Migrations -- SQLServer "Data Source=<Host>;User ID=<User>;Password=<Password>;Initial Catalog=ByteZoo.Blog.Data;Encrypt=false;"
dotnet ef database update -p ./Sources/ByteZoo.Blog.Common -- SQLServer "Data Source=<Host>;User ID=<User>;Password=<Password>;Initial Catalog=ByteZoo.Blog.Data;Encrypt=false;"