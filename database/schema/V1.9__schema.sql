alter table configuration.sds_query add column query_attributes varchar(500) null;

alter table configuration.sds_query add constraint configuration_sdsquery_queryattributes_ck check (char_length(trim(query_attributes)) > 0);

update configuration.sds_query
set query_attributes = 'nhsIDCode,o,postalAddress,postalCode,nhsOrgTypeCode' 
where query_name='GetOrganisationDetailsByOdsCode';
