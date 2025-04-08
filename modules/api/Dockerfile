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
COPY src/*.csproj ./src/

RUN dotnet restore
COPY . .
RUN dotnet build -c Debug -o /app/build

FROM build AS publish
WORKDIR /src/src
RUN dotnet publish -c Debug -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "gpconnect-appointment-checker.api.dll"]