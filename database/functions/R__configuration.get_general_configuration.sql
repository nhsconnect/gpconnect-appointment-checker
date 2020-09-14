create or replace function configuration.get_general_configuration
(
)
returns table
(
	product_name varchar(100),
 	product_version varchar(100),
 	max_num_weeks_search smallint
)
as $$
begin

	return query
	select
	    g.product_name,
	    g.product_version,
	    g.max_num_weeks_search
	from configuration.general g;
	
end;
$$ language plpgsql;

