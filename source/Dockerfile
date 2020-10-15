FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY gpconnect-appointment-checker.sln ./
COPY gpconnect-appointment-checker.UnitTest/*.csproj ./gpconnect-appointment-checker.UnitTest/
COPY gpconnect-appointment-checker.DTO/*.csproj ./gpconnect-appointment-checker.DTO/
COPY gpconnect-appointment-checker.DAL/*.csproj ./gpconnect-appointment-checker.DAL/
COPY gpconnect-appointment-checker.GPConnect/*.csproj ./gpconnect-appointment-checker.GPConnect/
COPY gpconnect-appointment-checker.SDS/*.csproj ./gpconnect-appointment-checker.SDS/
COPY gpconnect-appointment-checker/*.csproj ./gpconnect-appointment-checker/

RUN dotnet restore
COPY . .

WORKDIR /src/gpconnect-appointment-checker.UnitTest
RUN dotnet build -c Debug -o /app

WORKDIR /src/gpconnect-appointment-checker.DTO
RUN dotnet build -c Debug -o /app

WORKDIR /src/gpconnect-appointment-checker.DAL
RUN dotnet build -c Debug -o /app

WORKDIR /src/gpconnect-appointment-checker.GPConnect
RUN dotnet build -c Debug -o /app

WORKDIR /src/gpconnect-appointment-checker.SDS
RUN dotnet build -c Debug -o /app

WORKDIR /src/gpconnect-appointment-checker
RUN dotnet build -c Debug -o /app

FROM build AS publish
RUN dotnet publish -c Debug -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "gpconnect-appointment-checker.dll"]