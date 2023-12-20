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
		coalesce(b.multi_search_count, 0)	
	from
		(
			select 
				date_part('month', logged_date)::integer search_count_month, 
				date_part('year', logged_date)::integer search_count_year,
				sum(case when search_result_id is null then 1 else 0 end) as single_search_count
			from
				logging.spine_message sm
			where
				spine_message_type_id = 3
				and search_result_id is null
			group by
				date_part('month', logged_date)::integer, 
				date_part('year', logged_date)::integer
		) a
		left outer join
		(
			select
				date_part('month', sg.search_start_at)::integer search_count_month,
				date_part('year', sg.search_start_at)::integer search_count_year,
				count(*) multi_search_count
			from 
				application.search_group sg 
			group by
				date_part('month', sg.search_start_at)::integer,
				date_part('year', sg.search_start_at)::integer
		) b on a.search_count_month = b.search_count_month and a.search_count_year = b.search_count_year
	order by
		2 desc, 1 desc;
end;
$$ language plpgsql;