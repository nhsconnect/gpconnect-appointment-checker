alter table reporting.list add column interaction json;

update reporting.list set interaction = '[ "urn:nhs:names:services:gpconnect:structured:fhir:rest:read:metadata-1", "urn:nhs:names:services:gpconnect:documents:fhir:rest:read:metadata-1" ]' where interaction_id='urn:nhs:names:services:gpconnect:structured:fhir:rest:read:metadata-1';

delete from reporting.list where interaction_id = 'urn:nhs:names:services:gpconnect:documents:fhir:rest:read:metadata-1';

alter table reporting.list drop column interaction_id;