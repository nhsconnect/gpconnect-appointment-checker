FROM postgres:12.14
RUN apt-get update && apt-get install -y \
    postgresql-12-cron \
    apt-utils

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
ARG BUILD_TAG
ENV BUILD_TAG=$BUILD_TAG
RUN apt-get update && apt-get install -y libldap-2.4-2
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY gpconnect-appointment-checker.api.sln ./
COPY src/gpconnect-appointment-checker.api/gpconnect-appointment-checker.api.csproj ./src/gpconnect-appointment-checker.api/

# Restore the dependencies only for the main API project
RUN dotnet restore src/gpconnect-appointment-checker.api/gpconnect-appointment-checker.api.csproj

# Copy only the API project files (excluding tests)
COPY src/gpconnect-appointment-checker.api ./src/gpconnect-appointment-checker.api/

# Build the main application project (not the test projects)
RUN dotnet build src/gpconnect-appointment-checker.api/gpconnect-appointment-checker.api.csproj -c Debug -o /app/build

FROM build AS publish
WORKDIR /src/src/gpconnect-appointment-checker.api
RUN dotnet publish -c Debug -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "gpconnect-appointment-checker.api.dll"]
