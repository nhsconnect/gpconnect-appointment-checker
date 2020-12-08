alter table configuration.sds_query add column query_attributes varchar(500) null;

update configuration.sds_query set query_attributes = 'nhsIDCode,o,postalAddress,postalCode,nhsOrgTypeCode' where query_name='GetOrganisationDetailsByOdsCode';
update configuration.sds_query set query_attributes = 'nhsMHSPartyKey,uniqueIdentifier' where query_name='GetGpProviderAsIdByOdsCodeAndPartyKey';
update configuration.sds_query set query_attributes = 'nhsMHSEndPoint,nhsMHSPartyKey,uniqueIdentifier' where query_name='GetGpProviderEndpointAndPartyKeyByOdsCode';

alter table configuration.sds_query alter column query_attributes set not null;

alter table configuration.sds_query add constraint configuration_sdsquery_queryattributes_ck check (char_length(trim(query_attributes)) > 0);

