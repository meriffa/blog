# ByteZoo.Blog.Web Runtime Image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
RUN apt-get update && apt-get install -y procps
WORKDIR /ByteZoo.Blog.Web
COPY ./ByteZoo.Blog.Web.Publish .
ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_HTTP_PORTS=12080 \
    ASPNETCORE_HTTPS_PORTS=12443 \
    ASPNETCORE_Kestrel__Certificates__Default__Password=Passw0rd \
    ASPNETCORE_Kestrel__Certificates__Default__Path=/ByteZoo.Blog.Web/ByteZoo.Blog.Web.pfx
EXPOSE 12080/tcp
EXPOSE 12443/tcp
ENTRYPOINT ["dotnet", "ByteZoo.Blog.Web.dll"]