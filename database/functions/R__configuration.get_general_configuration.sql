drop function if exists configuration.get_general_configuration;

create function configuration.get_general_configuration
(
)
returns table
(
	product_name varchar(100),
 	product_version varchar(100),
 	max_num_weeks_search smallint,
 	log_retention_days integer,
	get_access_email_address varchar(100),
 	max_number_provider_codes_search smallint,
	max_number_consumer_codes_search smallint
)
as $$
begin
	return query
	select
		g.product_name,
		g.product_version,
		g.max_num_weeks_search,
		g.log_retention_days,
		g.get_access_email_address,
		g.max_number_provider_codes_search,
		g.max_number_consumer_codes_search
	from configuration.general g;	
end;
$$ language plpgsql;