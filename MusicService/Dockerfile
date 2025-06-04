FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5003 
ENV ASPNETCORE_URLS=http://+:5003

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

 
COPY ["MusicService/MusicService.csproj", "MusicService/"]
COPY ["Common/Common.csproj", "Common/"]

RUN dotnet restore "MusicService/MusicService.csproj"

WORKDIR "/src/Common"
COPY /Common .
WORKDIR "/src/MusicService"
COPY /MusicService .

RUN dotnet build "MusicService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MusicService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "MusicService.dll"]