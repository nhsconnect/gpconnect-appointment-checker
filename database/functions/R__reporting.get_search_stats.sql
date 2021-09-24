drop function if exists reporting.get_search_stats;

create function reporting.get_search_stats
(
)
returns table
(
    "Month" integer,
    "Year" integer,
    "Single Search Count" integer,
    "Multi Search Count" integer
)
as $$
begin
	return query
select
	multi_search_count_month,
	multi_search_count_year,
	single_search_count,
	multi_search_count
from
(
	select 
		date_part('month', logged_date)::integer as single_search_count_month, 
		date_part('year', logged_date)::integer as single_search_count_year, 
			count(*)::integer as single_search_count
		from
			logging.spine_message 
		where
			spine_message_type_id = 3 
			and search_result_id is null
		group by 
			date_part('month', logged_date),
			date_part('year', logged_date)) a
		left outer join 
		(
		select 
			date_part('month', search_start_at)::integer as multi_search_count_month, 
			date_part('year', search_start_at)::integer as multi_search_count_year,
			count(*)::integer as multi_search_count
		from 
			application.search_group
		group by 
			date_part('month', search_start_at),
			date_part('year', search_start_at)
) b on a.single_search_count_month = b.multi_search_count_month
	and a.single_search_count_year = b.multi_search_count_year
order by 
	multi_search_count_month desc, 
	multi_search_count_year desc;
end;
$$ language plpgsql;