select 
	use_ssp,
	ssp_hostname,
	sds_hostname,
	sds_port,
	sds_use_ldaps,
	organisation_id,
	party_key,
	asid,
	timeout_seconds
from configuration.get_spine_configuration
(
);
