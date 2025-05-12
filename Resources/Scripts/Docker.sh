#!/bin/bash

# Build projects
dotnet publish ./Sources/ByteZoo.Blog.App/ByteZoo.Blog.App.csproj -c Release -o ./Resources/Configurations/ByteZoo.Blog.App.Publish
dotnet publish ./Sources/ByteZoo.Blog.Web/ByteZoo.Blog.Web.csproj -c Release -o ./Resources/Configurations/ByteZoo.Blog.Web.Publish
dotnet dev-certs https --export-path ./Resources/Configurations/ByteZoo.Blog.Web.Publish/ByteZoo.Blog.Web.pfx -p Passw0rd

# Docker ByteZoo.Blog.App Container
docker build -t bytezoo.blog.app-image:1.0 -f ./Resources/Configurations/ByteZoo.Blog.App.Dockerfile ./Resources/Configurations
docker create --name ByteZoo.Blog.App-Container bytezoo.blog.app-image:1.0
docker start ByteZoo.Blog.App-Container
docker attach --sig-proxy=false ByteZoo.Blog.App-Container
docker exec -it ByteZoo.Blog.App-Container bash
docker stop ByteZoo.Blog.App-Container
docker run -it --rm --name ByteZoo.Blog.App-Container-Temp -v ~/ByteZoo.Blog.App.tmp:/tmp bytezoo.blog.app-image:1.0
docker run -it --rm --name ByteZoo.Blog.App-Container-Temp -v ~/ByteZoo.Blog.App.tmp:/tmp --entrypoint "dotnet" bytezoo.blog.app-image:1.0 ByteZoo.Blog.App.dll Concepts-Thread -t Thread -d "John Smith"

# Docker ByteZoo.Blog.Web Container
docker build -t bytezoo.blog.web-image:1.0 -f ./Resources/Configurations/ByteZoo.Blog.Web.Dockerfile ./Resources/Configurations
docker create --name ByteZoo.Blog.Web-Container bytezoo.blog.web-image:1.0
docker start ByteZoo.Blog.Web-Container
docker attach --sig-proxy=false ByteZoo.Blog.Web-Container
docker exec -it ByteZoo.Blog.Web-Container bash
docker stop ByteZoo.Blog.Web-Container
docker run -it --rm --name ByteZoo.Blog.Web-Container-Temp -p 12080:12080 -p 12443:12443 -v ~/ByteZoo.Blog.Web.tmp:/tmp bytezoo.blog.web-image:1.0

# Docker Compose ByteZoo.Blog Containers
docker compose -f ./Resources/Configurations/ByteZoo.Blog.yaml build
docker compose -f ./Resources/Configurations/ByteZoo.Blog.yaml ps -a
docker compose -f ./Resources/Configurations/ByteZoo.Blog.yaml up
docker compose -f ./Resources/Configurations/ByteZoo.Blog.yaml down