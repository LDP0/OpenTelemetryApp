# Use the .NET 8 SDK as the build environment
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY OpenTelemetryLibrary/*.csproj ./OpenTelemetryLibrary/
COPY OpenTelemetryAppServiceTwo/*.csproj ./OpenTelemetryAppServiceTwo/
RUN dotnet restore OpenTelemetryAppServiceTwo/OpenTelemetryAppServiceTwo.csproj

# Copy the entire directory and build the application
COPY OpenTelemetryLibrary/. ./OpenTelemetryLibrary/
COPY OpenTelemetryAppServiceTwo/. ./OpenTelemetryAppServiceTwo/
WORKDIR /app/OpenTelemetryAppServiceTwo
RUN dotnet publish -c Release -o out

# Use the .NET 8 runtime as the runtime environment
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/OpenTelemetryAppServiceTwo/out ./

# Expose the port for ServiceTwo
EXPOSE 80

# Set the entry point for the container
ENTRYPOINT ["dotnet", "OpenTelemetryAppServiceTwo.dll"]