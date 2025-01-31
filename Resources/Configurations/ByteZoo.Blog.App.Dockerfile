# ByteZoo.Blog.App Runtime Image
FROM mcr.microsoft.com/dotnet/runtime:9.0
WORKDIR /ByteZoo.Blog.App
COPY ../../ByteZoo.Blog.App.Publish .
ENTRYPOINT ["dotnet", "ByteZoo.Blog.App.dll", "Concepts-String"]