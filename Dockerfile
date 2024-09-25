# Stage 1
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["OpenTelemetryApp/OpenTelemetryApp.csproj", "OpenTelemetryApp/"]
RUN dotnet restore "OpenTelemetryApp/OpenTelemetryApp.csproj"
COPY . .
WORKDIR "/src/OpenTelemetryApp"
RUN dotnet build "OpenTelemetryApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OpenTelemetryApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OpenTelemetryApp.dll"]