alter table configuration.spine add sds_use_fhir_api boolean null;
update configuration.spine set sds_use_fhir_api = false;
alter table configuration.spine alter column sds_use_fhir_api set not null;

create table configuration.fhir_api_query
(
    query_name varchar(100) not null,
    query_text varchar(1000) not null,
    
    constraint configuration_fhirapiquery_queryname_pk primary key (query_name),
    constraint configuration_fhirapiquery_queryname_ck check (char_length(trim(query_name)) > 0),
    constraint configuration_fhirapiquery_querytext_ck check (char_length(trim(query_text)) > 0)
);

create unique index configuration_fhirapiquery_queryname_ix on configuration.fhir_api_query (lower(query_name));

insert into configuration.fhir_api_query
(
    query_name,
    query_text
)
values
(
	'GetAccreditedSystemDetailsFromSDS',
	'/spine-directory/FHIR/R4/Device?organization=https://fhir.nhs.uk/Id/ods-organization-code|{odsCode}&identifier=https://fhir.nhs.uk/Id/nhsServiceInteractionId|urn:nhs:names:services:gpconnect:fhir:rest:read:metadata-1&identifier=https://fhir.nhs.uk/Id/nhsMhsPartyKey|{partyKey}'
),
(
	'GetOrganisationDetailsByOdsCode',
	'/STU3/Organization/{odsCode}'
),
(
	'GetRoutingReliabilityDetailsFromSDS',
	'/spine-directory/FHIR/R4/Endpoint?organization=https://fhir.nhs.uk/Id/ods-organization-code|{odsCode}&identifier=https://fhir.nhs.uk/Id/nhsServiceInteractionId|urn:nhs:names:services:gpconnect:fhir:rest:read:metadata-1'
),
(
	'GetAccreditedSystemDetailsForConsumerFromSDS',
	'/spine-directory/FHIR/R4/Device?organization=https://fhir.nhs.uk/Id/ods-organization-code|{odsCode}&identifier=https://fhir.nhs.uk/Id/nhsServiceInteractionId|urn:nhs:names:services:gpconnect:fhir:rest:read:metadata-1'
);

alter table configuration.spine add spine_fhir_api_directory_services_fqdn varchar(100) null;
update configuration.spine set spine_fhir_api_directory_services_fqdn = 'https://test.com';
alter table configuration.spine alter column spine_fhir_api_directory_services_fqdn set not null;

alter table configuration.spine add spine_fhir_api_systems_register_fqdn varchar(100) null;
update configuration.spine set spine_fhir_api_systems_register_fqdn = 'https://test.com';
alter table configuration.spine alter column spine_fhir_api_systems_register_fqdn set not null;

alter table configuration.spine_message_type drop constraint configuration_spinemessagetype_interactionid_ck;
alter table configuration.spine_message_type alter column interaction_id drop not null;

insert into configuration.spine_message_type 
(
	spine_message_type_id,
	spine_message_type_name
) 
values 
(	4, 
	'Spine Directory Service - FHIR API SDS query'
),
(
	5, 
	'Spine Directory Service - FHIR API Organisation query'
);

alter table configuration.spine add spine_fhir_api_key varchar(100) null;
update configuration.spine set spine_fhir_api_fqdn = 'https://test.com';

update application.email_template set body='A new user create account form has been posted with the following details:

Email Address: <email_address>

Job Role: <job_role>

Organisation: <organisation_name>

Access Reason: <access_reason>

<url>' where email_template_id='3';

update application.email_template set body='Your access to the GP Connect Appointment Checker Tool has been removed. 

If you still need access, please email <address> with your name, email address, organisation, role and reason.

<url>' where email_template_id='2';

update application.email_template set body='You have been granted access to the GP Connect Appointment Checker Tool.

To use the tool, go to <url> and follow the instructions. Sign on with the email address that this email was sent to, and your usual password.

For more help, use the "Help" link which appears on the Appointment Checker Tool pages.

<url>' where email_template_id='1';

grant select, insert, update on all tables in schema configuration to app_user;
grant select, update on all sequences in schema configuration to app_user;
grant execute on all functions in schema configuration to app_user;