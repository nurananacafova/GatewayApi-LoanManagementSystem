﻿FROM mcr.microsoft.com/dotnet/sdk:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["GatewayAPI/GatewayAPI.csproj", "GatewayAPI/"]
RUN dotnet restore "GatewayAPI/GatewayAPI.csproj"
COPY . .
WORKDIR "/src/GatewayAPI"
RUN dotnet build "GatewayAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GatewayAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GatewayAPI.dll"]



