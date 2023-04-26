FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /src

COPY gpconnect-appointment-checker.sln ./
COPY src/*.csproj ./src/

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
