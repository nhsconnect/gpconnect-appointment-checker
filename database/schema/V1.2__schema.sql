-- remove unused columns
alter table application.organisation drop column is_gpconnect_consumer;
alter table application.organisation drop column is_gpconnect_provider;

-- remove unused sds queries
delete from configuration.sds_query where query_name = 'OrganisationHasAppointmentsConsumerSystemByOdsCode';
delete from configuration.sds_query where query_name = 'OrganisationHasAppointmentsProviderSystemByOdsCode';

-- remove audit entries for logging change in consumer/provider status
delete from audit.entry_type where entry_type_id in (10, 11);

-- add new spine timeout column
alter table configuration.spine add column timeout_seconds integer null;
alter table configuration.spine add constraint configuration_spine_timeoutseconds_ck check (timeout_seconds > 0);
update configuration.spine set timeout_seconds = 30;
alter table configuration.spine alter column timeout_seconds set not null;
