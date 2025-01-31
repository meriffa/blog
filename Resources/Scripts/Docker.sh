#!/bin/bash

# ByteZoo.Blog.App Container
dotnet publish ./Sources/ByteZoo.Blog.App/ByteZoo.Blog.App.csproj -c Release -o ./ByteZoo.Blog.App.Publish
docker build -t bytezoo.blog.app-image:1.0 -f ./Resources/Configurations/ByteZoo.Blog.App.Dockerfile .
docker create --name ByteZoo.Blog.App-Container bytezoo.blog.app-image:1.0
docker start ByteZoo.Blog.App-Container
docker attach --sig-proxy=false ByteZoo.Blog.App-Container
docker exec -it ByteZoo.Blog.App-Container bash
docker stop ByteZoo.Blog.App-Container
docker run -it --rm bytezoo.blog.app-image:1.0
docker run -it --rm --entrypoint "dotnet" bytezoo.blog.app-image:1.0 ByteZoo.Blog.App.dll Concepts-Thread -t Thread -d "John Smith"

# ByteZoo.Blog.Web Container
dotnet publish ./Sources/ByteZoo.Blog.Web/ByteZoo.Blog.Web.csproj -c Release -o ./ByteZoo.Blog.Web.Publish
docker build -t bytezoo.blog.web-image:1.0 -f ./Resources/Configurations/ByteZoo.Blog.Web.Dockerfile .
docker run -it --rm -p 7080:8080 -p 7443:8443 bytezoo.blog.web-image:1.0