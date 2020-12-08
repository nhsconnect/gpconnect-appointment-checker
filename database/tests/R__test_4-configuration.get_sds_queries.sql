select 
	query_name,
	search_base,
	query_text,
	query_attributes
from configuration.get_sds_queries
(
);
