FROM public.ecr.aws/lambda/dotnet:7 as base
FROM mcr.microsoft.com/dotnet/sdk:7.0 as build

WORKDIR /src

COPY gpconnect-appointment-checker.function.sln ./
COPY src/*.csproj ./src/

RUN dotnet restore
COPY . .
RUN dotnet build -c Debug -o /app/build

FROM build AS publish
RUN dotnet publish \
    --framework net7.0 \
    --runtime linux-x64 \
    --self-contained true \
    --configuration Debug \
    --output /app/publish \
    -p:PublishReadyToRun=true

FROM base AS final
WORKDIR /var/task
COPY --from=publish /app/publish .

# CMD [ "GpConnect.AppointmentChecker.Function::GpConnect.AppointmentChecker.Function.CapabilityReportScheduledEventFunction::FunctionHandler" ]