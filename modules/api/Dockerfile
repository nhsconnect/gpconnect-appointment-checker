FROM postgres:12.14
RUN apt-get update && apt-get install -y \
    postgresql-12-cron \
    apt-utils

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
ARG BUILD_TAG
ENV BUILD_TAG=$BUILD_TAG
RUN apt-get update && apt-get install -y libldap-2.4-2
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY src/*.csproj ./src/

# Restore only the API project
RUN dotnet restore ./src/gpconnect-appointment-checker.api.csproj

COPY . .

# Build only the API project
RUN dotnet build ./src/gpconnect-appointment-checker.api.csproj -c Debug -o /app/build

FROM build AS publish
WORKDIR /src/src

# Publish only the API project
RUN dotnet publish gpconnect-appointment-checker.api.csproj -c Debug -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "gpconnect-appointment-checker.api.dll"]
