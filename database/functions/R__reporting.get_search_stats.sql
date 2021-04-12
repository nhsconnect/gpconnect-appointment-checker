drop function if exists reporting.get_search_stats;

create function reporting.get_search_stats
(
)
returns table
(
    "Week" integer,
    "Month" integer,
    "Year" integer,
    "Count" integer
)
as $$
begin

    return query
    select 
        date_part('week', logged_date)::integer AS "Week",
        date_part('month', logged_date)::integer AS "Month", 
        date_part('year', logged_date)::integer AS "Year", 
        count(*)::integer AS "Count"
    from logging.spine_message 
    where spine_message_type_id = 3 
    group by 
        date_part('week', logged_date),
        date_part('year', logged_date),
        date_part('month', logged_date)
    order by 
        date_part('year', logged_date) desc,
        date_part('week', logged_date) desc;

end;
$$ language plpgsql;