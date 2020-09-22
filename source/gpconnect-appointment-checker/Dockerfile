FROM mcr.microsoft.com/dotnet/core/aspnet:3.1.8-alpine3.12 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

ENV GPCONNECTAPPOINTMENTCHECKER_LdapQueries_GetGpProviderAsIdByOdsCodeAndPartyKey="(&(nhsIDCode={odsCode})(objectClass=nhsAs)(nhsMhsPartyKey={partyKey}))"
ENV GPCONNECTAPPOINTMENTCHECKER_LdapQueries_GetGpProviderEndpointAndPartyKeyByOdsCode="(&(nhsIDCode={odsCode})(objectClass=nhsMhs)(nhsMhsSvcIA=urn:nhs:names:services:gpconnect:structured:fhir:rest:read:metadata-1))"
ENV GPCONNECTAPPOINTMENTCHECKER_LdapQueries_GetOrganisationDetailsByOdsCode="(uniqueidentifier={odsCode})"
ENV GPCONNECTAPPOINTMENTCHECKER_LdapQueries_OrganisationHasAppointmentsConsumerSystemByOdsCode="(&(nhsIDCode={odsCode})(objectClass=nhsAs)(nhsAsSvcIA=urn:nhs:names:services:gpconnect:structured:fhir:rest:read:metadata-1))"
ENV GPCONNECTAPPOINTMENTCHECKER_LdapQueries_OrganisationHasAppointmentsProviderSystemByOdsCode="(&(nhsIDCode={odsCode})(objectClass=nhsMhs)(nhsMhsSvcIA=urn:nhs:names:services:gpconnect:structured:fhir:rest:read:metadata-1))"
ENV GPCONNECTAPPOINTMENTCHECKER_LdapQueries_OrganisationsSearchBase="ou=organisations, o=nhs"
ENV GPCONNECTAPPOINTMENTCHECKER_LdapQueries_ServicesSearchBase="ou=services, o=nhs"
ENV GPCONNECTAPPOINTMENTCHECKER_OAuthClientId="ClientId"
ENV GPCONNECTAPPOINTMENTCHECKER_OAuthClientSecret="ClientSecret"
ENV GPCONNECTAPPOINTMENTCHECKER_OAuthPathString="/signin-nhssso"

FROM mcr.microsoft.com/dotnet/core/sdk:3.1.402-alpine3.12 AS build
WORKDIR /src
COPY ["gpconnect-appointment-checker/gpconnect-appointment-checker.csproj", "gpconnect-appointment-checker/"]
RUN dotnet restore "gpconnect-appointment-checker/gpconnect-appointment-checker.csproj"
COPY . .
WORKDIR "/src/gpconnect-appointment-checker"
RUN dotnet build "gpconnect-appointment-checker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "gpconnect-appointment-checker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "gpconnect-appointment-checker.dll"]

