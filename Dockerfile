# Stage 1
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["OpenTelemetryTracesApp/OpenTelemetryTracesApp.csproj", "OpenTelemetryTracesApp/"]
RUN dotnet restore "OpenTelemetryTracesApp/OpenTelemetryTracesApp.csproj"
COPY . .
WORKDIR "/src/OpenTelemetryTracesApp"
RUN dotnet build "OpenTelemetryTracesApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OpenTelemetryTracesApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OpenTelemetryTracesApp.dll"]