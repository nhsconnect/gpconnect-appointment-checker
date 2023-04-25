FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /src

COPY gpconnect-appointment-checker.api.sln ./
COPY src/*.csproj ./src/

RUN dotnet restore
COPY . .
RUN dotnet build -c Debug -o /app/build

FROM build AS publish
WORKDIR /src/src
RUN dotnet publish -c Debug -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "gpconnect-appointment-checker.api.dll"]