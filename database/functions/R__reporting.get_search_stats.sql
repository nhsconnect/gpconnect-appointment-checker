drop function if exists reporting.get_search_stats;

create function reporting.get_search_stats
(
)
returns table
(
    "Month" integer,
    "Year" integer,
    "Single Search Count" bigint,
    "Multi Search Count" bigint
)
as $$
begin
	return query
	select
		a.search_count_month,
		a.search_count_year,
		coalesce(a.single_search_count, 0),
		coalesce(b.multiple_search_count, 0)
	from 
	(
		select 
			date_part('month', logged_date)::integer search_count_month, 
			date_part('year', logged_date)::integer search_count_year,
			count(*) single_search_count
		from
			logging.spine_message	
		where
			spine_message_type_id = 3 
			and search_result_id is null
		group by 
			1,2
	) a left outer join
	(
		select 
			date_part('month', logged_date)::integer search_count_month, 
			date_part('year', logged_date)::integer search_count_year,
			count(*) multiple_search_count
		from
			logging.spine_message sm
			inner join application.search_result sr on sm.search_result_id = sr.search_result_id
			inner join application.search_group sg on sr.search_group_id = sg.search_group_id
		where
			sm.spine_message_type_id = 3 
			and sm.search_result_id is not null
		group by 
			1,2
	) b on a.search_count_month = b.search_count_month
	and a.search_count_year = b.search_count_year	
	order by
		2 desc, 1 desc;
end;
$$ language plpgsql;