drop function if exists reporting.get_multi_search_stats;

create function reporting.get_multi_search_stats
(
)
returns table
(
	"Number of Provider ODS Codes" integer,
	"Number of Consumer ODS Codes" integer,
	"Count" bigint,
	"Average Total Response Time" interval,
	"Average Response Per Code" interval
)
as $$
begin
	return query
	select
		array_length(string_to_array(provider_ods_textbox, ' '), 1),
		array_length(string_to_array(consumer_ods_textbox, ' '), 1),
		count(*),
		date_trunc('millisecond', avg(search_end_at - search_start_at)),
		date_trunc('millisecond', avg(search_end_at - search_start_at) / 
			(array_length(string_to_array(provider_ods_textbox, ' '), 1) + array_length(string_to_array(consumer_ods_textbox, ' '), 1)))
	from 
		application.search_group
	group by 
		array_length(string_to_array(provider_ods_textbox, ' '), 1),
		array_length(string_to_array(consumer_ods_textbox, ' '), 1)
	order by 
		3 desc,1,2;
end;
$$ language plpgsql;