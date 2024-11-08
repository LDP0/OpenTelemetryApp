# Use the .NET 8 SDK as the build environment
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY OpenTelemetryLibrary/*.csproj ./OpenTelemetryLibrary/
COPY OpenTelemetryAppServiceOne/*.csproj ./OpenTelemetryAppServiceOne/
RUN dotnet restore OpenTelemetryAppServiceOne/OpenTelemetryAppServiceOne.csproj

# Copy the entire directory and build the application
COPY OpenTelemetryLibrary/. ./OpenTelemetryLibrary/
COPY OpenTelemetryAppServiceOne/. ./OpenTelemetryAppServiceOne/
WORKDIR /app/OpenTelemetryAppServiceOne
RUN dotnet publish -c Release -o out

# Use .NET 8 runtime as the runtime environment
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/OpenTelemetryAppServiceOne/out ./

# Expose the port for ServiceOne
EXPOSE 80

# Set the entry point for the container
ENTRYPOINT ["dotnet", "OpenTelemetryAppServiceOne.dll"]