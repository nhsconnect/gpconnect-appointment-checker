select 
	use_ssp,
	ssp_hostname,
	sds_hostname,
	sds_port,
	sds_use_ldaps,
	organisation_id,
	party_key,
	asid,
	timeout_seconds,
	client_cert,
	client_private_key
from configuration.get_spine_configuration
(
);
