alter table reporting.list add column interaction_id character varying (100);

insert into reporting.list(report_name, interaction_id) VALUES ('Access Record: Structured','urn:nhs:names:services:gpconnect:structured:fhir:rest:read:metadata-1');

ALTER TABLE IF EXISTS reporting.list DROP CONSTRAINT IF EXISTS reporting_list_interactionid_functionname_ck;

ALTER TABLE IF EXISTS reporting.list ADD CONSTRAINT reporting_list_interactionid_functionname_ck CHECK (interaction_id is not null and function_name is null OR interaction_id is null and function_name is not null);
