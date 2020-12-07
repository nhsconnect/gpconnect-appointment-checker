alter table configuration.sds_query add column query_attributes varchar(500) null;
update configuration.sds_query set query_attributes = 'nhsIDCode,o,postalAddress,postalCode,nhsOrgTypeCode' where query_name='GetOrganisationDetailsByOdsCode'
