FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /src

COPY gpconnect-appointment-checker.sln ./
COPY src/gpconnect-appointment-checker.Console/*.csproj ./src/gpconnect-appointment-checker.Console/
COPY src/gpconnect-appointment-checker.Helpers/*.csproj ./src/gpconnect-appointment-checker.Helpers/
COPY src/gpconnect-appointment-checker.DTO/*.csproj ./src/gpconnect-appointment-checker.DTO/
COPY src/gpconnect-appointment-checker.DAL/*.csproj ./src/gpconnect-appointment-checker.DAL/
COPY src/gpconnect-appointment-checker.GPConnect/*.csproj ./src/gpconnect-appointment-checker.GPConnect/
COPY src/gpconnect-appointment-checker.SDS/*.csproj ./src/gpconnect-appointment-checker.SDS/
COPY src/gpconnect-appointment-checker.AWSLambda/*.csproj ./src/gpconnect-appointment-checker.AWSLambda/
COPY src/gpconnect-appointment-checker/*.csproj ./src/gpconnect-appointment-checker/

RUN dotnet restore
COPY . .

WORKDIR /src
RUN dotnet build -c Debug -o /app

FROM build AS publish
RUN dotnet publish -c Debug -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .

RUN sed -i "s|DEFAULT@SECLEVEL=2|DEFAULT@SECLEVEL=1|g" /etc/ssl/openssl.cnf
# RUN sed -i "s|SHARED_PRELOAD_LIBRARIES=PG_CRON|g" /var/lib/postgresql/data/postgresql.conf

ENTRYPOINT ["dotnet", "gpconnect-appointment-checker.dll"]
