create or replace function configuration.get_spine_configuration
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
    party_key varchar(20),
    asid varchar(20),
    timeout_seconds integer,
    client_cert varchar(8000),
    client_private_key varchar(8000)
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
	    s.party_key,
	    s.asid,
	    s.timeout_seconds,
	    s.client_cert,
	    s.client_private_key
	from configuration.spine s;
	
end;
$$ language plpgsql;
