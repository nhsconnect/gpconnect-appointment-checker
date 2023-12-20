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

-- add new sso metadata endpoint column
alter table configuration.sso add column metadata_endpoint varchar null;
alter table configuration.sso add constraint configuration_sso_metadataendpoint_ck check (char_length(trim(metadata_endpoint)) > 0);
update configuration.sso set metadata_endpoint = 'https://metadata_endpoint';
alter table configuration.sso alter column metadata_endpoint set not null;

-- add new sso end session endpoint column
alter table configuration.sso add column endsession_endpoint varchar null;
alter table configuration.sso add constraint configuration_sso_endsessionendpoint_ck check (char_length(trim(endsession_endpoint)) > 0);
update configuration.sso set endsession_endpoint = 'https://endsession_endpoint';
alter table configuration.sso alter column endsession_endpoint set not null;

-- add new log retention period column
alter table configuration.general add column log_retention_days integer null;
update configuration.general set log_retention_days = 365;
alter table configuration.general add constraint configuration_general_logretentiondays_ck check (log_retention_days > 0);
alter table configuration.general alter column log_retention_days set not null;

-- change data type of error log logged column
alter table logging.error_log alter column logged type timestamp using logged::timestamp without time zone;
alter table logging.error_log alter column logged set not null;