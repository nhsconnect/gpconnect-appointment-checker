drop function if exists configuration.get_spine_configuration;

create function configuration.get_spine_configuration
(
)
returns table
(
    use_ssp boolean,
    ssp_hostname varchar(100),
    sds_hostname varchar(100),
    sds_port integer,
    sds_use_ldaps boolean,
    organisation_id integer,
    ods_code varchar(10),
    organisation_name varchar(100),
    party_key varchar(20),
    asid varchar(20),
    timeout_seconds integer,
    client_cert varchar(8000),
    client_private_key varchar(8000),
    server_ca_certchain varchar(8000),
    sds_use_mutualauth boolean,
    spine_fqdn varchar(100),
    sds_tls_version varchar(10),
    sds_use_fhir_api boolean,
    spine_fhir_api_directory_services_fqdn varchar(100),
    spine_fhir_api_systems_register_fqdn varchar(100),
    spine_fhir_api_key varchar(100)
)
as $$
begin

	return query
	select
	    s.use_ssp,
	    s.ssp_hostname,
	    s.sds_hostname,
	    s.sds_port,
	    s.sds_use_ldaps,
	    s.organisation_id,
	    o.ods_code,
	    o.organisation_name,
	    s.party_key,
	    s.asid,
	    s.timeout_seconds,
	    s.client_cert,
	    s.client_private_key,
	    s.server_ca_certchain,
	    s.sds_use_mutualauth,
	    s.spine_fqdn,
	    s.sds_tls_version,
	    s.sds_use_fhir_api,
	    s.spine_fhir_api_directory_services_fqdn,
	    s.spine_fhir_api_systems_register_fqdn,
	    s.spine_fhir_api_key
	from configuration.spine s
	inner join application.organisation o on s.organisation_id = o.organisation_id;	
end;
$$ language plpgsql;