drop function if exists reporting.get_user_stats;

create function reporting.get_user_stats
(
)
returns table
(
    stat_id integer,
    stat_name text,
    stat_value integer
)
as $$
declare
    _total_authorised_users integer;
    _total_unauthorised_users integer;
    _users_loggedon_month integer;
    _users_loggedon_week integer;
begin

    select 
        count(*) into _total_authorised_users
    from application."user" 
    where is_authorised = true;

    select 
        count(*) into _total_unauthorised_users
    from application."user" 
    where is_authorised = false;

    select 
        count(*) into _users_loggedon_month
    from application."user" 
    where last_logon_date > (now() - interval '1 month');

    select 
        count(*) into _users_loggedon_week
    from application."user" 
    where last_logon_date > (now() - interval '1 week');

    return query
    select
        1 as stat_id,
        'total_authorised_users' as stat_name,
        _total_authorised_users as stat_value
    union
    select
        2 as stat_id,
        'total_unauthorised_users' as stat_name,
        _total_unauthorised_users as stat_value
    union
    select
        3 as stat_id,
        'users_loggedon_month' as stat_name,
        _users_loggedon_month as stat_value
    union
    select
        4 as stat_id,
        'users_loggedon_week' as stat_name,
        _users_loggedon_week as stat_value
    order by 
        stat_id asc;
end;
$$ language plpgsql;