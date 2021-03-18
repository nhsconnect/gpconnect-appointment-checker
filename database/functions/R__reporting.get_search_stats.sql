drop function if exists reporting.get_search_stats;

create function reporting.get_search_stats
(
)
returns table
(
    week integer,
    month integer,
    year integer,
    count integer
)
as $$
begin

    return query
    select 
        date_part('week', logged_date)::integer as week,
        date_part('month', logged_date)::integer as month, 
        date_part('year', logged_date)::integer as year, 
        count(*)::integer as count
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