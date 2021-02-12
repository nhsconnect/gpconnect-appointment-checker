drop function if exists reporting.get_transaction_stats;

create function reporting.get_transaction_stats
(
)
returns table
(
    month integer,
    year integer,
    interaction_id text,
    count integer
)
as $$
begin

    return query
    select 
        msg.month::integer,
        msg.year::integer,
        sm.interaction_id::text,
        msg.count::integer
    from
    (
        select 
            date_part('month', logged_date) as month, 
            date_part('year', logged_date) as year, 
            spine_message_type_id,
            count(*) 
        from logging.spine_message
        group by 
            spine_message_type_id,
            date_part('year', logged_date),
            date_part('month', logged_date)
    ) msg
    inner join configuration.spine_message_type sm on msg.spine_message_type_id = sm.spine_message_type_id
    order by 
        msg.year desc,
        msg.month desc,
        msg.spine_message_type_id asc;

end;
$$ language plpgsql;