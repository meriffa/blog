# ByteZoo.Blog.Web Runtime Image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /ByteZoo.Blog.Web
COPY ../../ByteZoo.Blog.Web.Publish .
ENV ASPNETCORE_HTTP_PORTS=8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "ByteZoo.Blog.Web.dll"]