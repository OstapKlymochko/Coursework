FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5001 
ENV ASPNETCORE_URLS=http://+:5001

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

 
COPY ["AuthService/AuthService.csproj", "AuthService/"]
COPY ["Common/Common.csproj", "Common/"]

RUN dotnet restore "AuthService/AuthService.csproj"

WORKDIR "/src/Common"
COPY /Common .
WORKDIR "/src/AuthService"
COPY /AuthService .

RUN dotnet build "AuthService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AuthService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "AuthService.dll"]