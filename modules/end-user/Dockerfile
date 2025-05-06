# Use the ASP.NET Core runtime base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
ARG BUILD_TAG
ENV BUILD_TAG=$BUILD_TAG
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the .NET SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy only the necessary project files first for efficient caching
COPY src/gpconnect-appointment-checker.Helpers/gpconnect-appointment-checker.Helpers.csproj src/gpconnect-appointment-checker.Helpers/
COPY src/gpconnect-appointment-checker.AWSLambda/gpconnect-appointment-checker.AWSLambda.csproj src/gpconnect-appointment-checker.AWSLambda/
COPY src/gpconnect-appointment-checker/gpconnect-appointment-checker.csproj src/gpconnect-appointment-checker/

# Restore dependencies
RUN dotnet restore src/gpconnect-appointment-checker/gpconnect-appointment-checker.csproj

# Copy the rest of the application source code
COPY src/ src/

# Build the application
RUN dotnet build src/gpconnect-appointment-checker/gpconnect-appointment-checker.csproj -c Release -o /app/build

# Publish the application
RUN dotnet publish src/gpconnect-appointment-checker/gpconnect-appointment-checker.csproj -c Release -o /app/publish --no-restore

# Final stage with only necessary runtime dependencies
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# Run the application
ENTRYPOINT ["dotnet", "gpconnect-appointment-checker.dll"]
