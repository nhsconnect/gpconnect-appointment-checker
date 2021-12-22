drop function if exists configuration.get_fhir_api_queries;

create function configuration.get_fhir_api_queries
(
)
returns table
(
	query_name varchar(100),
	query_text varchar(1000)
)
as $$
begin

	return query
	select
		q.query_name,
		q.query_text
	from configuration.fhir_api_query q;
	
end;
$$ language plpgsql;

