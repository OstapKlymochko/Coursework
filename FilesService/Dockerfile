FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5004 
ENV ASPNETCORE_URLS=http://+:5004

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

 
COPY ["FilesService/FilesService.csproj", "FilesService/"]
COPY ["Common/Common.csproj", "Common/"]

RUN dotnet restore "FilesService/FilesService.csproj"

WORKDIR "/src/Common"
COPY /Common .
WORKDIR "/src/FilesService"
COPY /FilesService .

RUN dotnet build "FilesService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FilesService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "FilesService.dll"]