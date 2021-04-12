drop function if exists reporting.get_user_stats;

create function reporting.get_user_stats
(
)
returns table
(
    "Name" text,
    "Count" integer
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
        'Total Authorised Users' AS "Type",
        _total_authorised_users AS "Count"
    union
    select
        'Total Unauthorised Users' AS "Type",
        _total_unauthorised_users AS "Count"
    union
    select
        'Users Logged on Month' AS "Type",
        _users_loggedon_month AS "Count"
    union
    select
        'Users Logged on Week' AS "Type",
        _users_loggedon_week AS "Count"
	
	ORDER BY 1;
end;
$$ language plpgsql;