#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
# For debugging uncomment ln 6 and 22

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
#EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["GeoFinder/GeoFinder.csproj", "GeoFinder/"]
COPY ["GeoFinder.Infrastructure/GeoFinder.Infrastructure.csproj", "GeoFinder.Infrastructure/"]
RUN dotnet restore "GeoFinder/GeoFinder.csproj"
COPY . .
WORKDIR "/src/GeoFinder"
RUN dotnet build "GeoFinder.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GeoFinder.csproj" -c Release -o /app/publish

FROM base AS final
COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "GeoFinder.dll"]
CMD ASPNETCORE_URLS=http://*:$PORT dotnet GeoFinder.dll