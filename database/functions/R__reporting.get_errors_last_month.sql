drop function if exists reporting.get_errors_last_month;

create function reporting.get_errors_last_month
(
)
returns table
(
    "Error Message" text,
    "Error Logger" text,
    "Number of Occurrences" integer,
    "Last Occurred" timestamp
)
as $$
begin
    return query
    select 
        el.message::text AS "Error Message", 
        el.logger::text AS "Error Logger", 
        count(*)::integer AS "Number of Occurrences",
        max(el.logged) AS "Last Occurred"
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
