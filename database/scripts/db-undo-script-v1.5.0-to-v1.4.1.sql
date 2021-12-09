/*
	Database rollback script
	from Appointment Checker v1.5.0
	to Appointment Checker v1.4.1

	Undos patches V4.8 to V5.5, including flyway_schema_history table
*/


/*
	patch V4.8 - (blank)
*/

-- (nothing to do)

/*
	patch V4.9 - adds column sds_use_fhir_api
*/
alter table configuration.spine drop column sds_use_fhir_api;

/*
	patch V5.0 - adds configuration.fhir_api_query table
				 adds index configuration_fhirapiquery_queryname_ix
*/
drop index configuration.configuration_fhirapiquery_queryname_ix;
drop table configuration.fhir_api_query;

/*
	patch V5.1 - adds new spine message types
				 drops constraint configuration_spinemessagetype_interactionid_ck
				 removes not null from interaction_id
*/
delete from logging.spine_message where spine_message_type_id in (4, 5);
delete from configuration.spine_message_type where spine_message_type_id in (4, 5);
alter table configuration.spine_message_type add constraint configuration_spinemessagetype_interactionid_ck check (char_length(trim(interaction_id)) > 0);
alter table configuration.spine_message_type alter column interaction_id set not null;

/*
	patch V5.2 - adds column spine_fhir_api_key
*/
alter table configuration.spine drop column spine_fhir_api_key;

/*
	patch V5.3 - updates email templates
*/

update application.email_template set body='A new user create account form has been posted with the following details:

Email Address: <email_address>

Job Role: <job_role>

Organisation: <organisation_name>

Access Reason: <access_reason>
' where email_template_id='3';


update application.email_template set body='Your access to the GP Connect Appointment Checker Tool has been removed. 

If you still need access, please email <address> with your name, email address, organisation, role and reason.' where email_template_id='2';


update application.email_template set body='You have been granted access to the GP Connect Appointment Checker Tool.

To use the tool, go to <url> and follow the instructions. Sign on with the email address that this email was sent to, and your usual password.

For more help, use the "Help" link which appears on the Appointment Checker Tool pages.' where email_template_id='1';

/*
	patch V5.4 - adds column spine_fhir_api_directory_services_fqdn
				 adds column spine_fhir_api_systems_register_fqdn

*/
alter table configuration.spine drop column spine_fhir_api_directory_services_fqdn;
alter table configuration.spine drop column spine_fhir_api_systems_register_fqdn;

/*
	patch V5.5 - updates to tables already dropped
*/

-- (nothing to do)


/*
	clean up public.flyway_schema_history
*/

delete from public.flyway_schema_history where script in
(
	'schema/V4.8__schema.sql',
	'schema/V4.9__schema.sql',
	'schema/V5.0__schema.sql',
	'schema/V5.1__schema.sql',
	'schema/V5.2__schema.sql',
	'schema/V5.3__schema.sql',
	'schema/V5.4__schema.sql',
	'schema/V5.5__schema.sql'
);

delete from public.flyway_schema_history 
where script =	'functions/R__configuration.get_fhir_api_queries.sql';

delete from public.flyway_schema_history 
where script = 'functions/R__configuration.get_spine_configuration.sql'
and checksum = -1021888514;