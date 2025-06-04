FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5002 
ENV ASPNETCORE_URLS=http://+:5002

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

 
COPY ["UserService/UserService.csproj", "UserService/"]
COPY ["Common/Common.csproj", "Common/"]

RUN dotnet restore "UserService/UserService.csproj"

WORKDIR "/src/Common"
COPY /Common .
WORKDIR "/src/UserService"
COPY /UserService .

RUN dotnet build "UserService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UserService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "UserService.dll"]