#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["text-game-server/text-game-server.csproj", "text-game-server/"]
RUN dotnet restore "text-game-server/text-game-server.csproj"
COPY . .
WORKDIR "/src/text-game-server"
RUN dotnet build "text-game-server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "text-game-server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "text-game-server.dll"]