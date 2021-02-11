drop function if exists reporting.get_errors_last_month;

create function reporting.get_errors_last_month
(
)
returns table
(
    error_message text,
    logger text,
    num_occurrences integer,
    last_ocurred timestamp
)
as $$
begin

    return query
    select 
        el.message::text as error_message, 
        el.logger::text, 
        count(*)::integer as num_occurrences,
        max(el.logged) as last_occurred
    from logging.error_log el
    where el.level = 'ERROR'
    and el.logged > now() - 1 * interval '1 month' 
    group by 
        el.message, 
        el.logger
    order by
        count(*) desc;

end;
$$ language plpgsql;
