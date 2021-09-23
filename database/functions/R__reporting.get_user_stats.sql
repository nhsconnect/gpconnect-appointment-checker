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
    _total_pending_users integer;
    _total_authorised_users integer;
    _total_deauthorised_users integer;
    _total_requestdenied_users integer;
    _users_loggedon_month integer;
    _users_loggedon_week integer;
    _total_multisearchenabled_users integer;
    _total_adminaccess_users integer;
    _total_orgtypesearchenabled_users integer;
begin

	select 
		count(*) into _total_pending_users
	from 
		application."user" 
	where
		user_account_status_id = 1;
		
	select 
		count(*) into _total_authorised_users
	from 
		application."user" 
	where
		user_account_status_id = 2;
		
	select 
		count(*) into _total_deauthorised_users
	from 
		application."user" 
	where
		user_account_status_id = 3;

	select 
		count(*) into _total_requestdenied_users
	from 
		application."user" 
	where
		user_account_status_id = 4;

    select 
        count(*) into _users_loggedon_month
    from application."user" 
    where last_logon_date > (now() - interval '1 month');

    select 
        count(*) into _users_loggedon_week
    from application."user" 
    where last_logon_date > (now() - interval '1 week');

	select 
		count(*) into _total_multisearchenabled_users
	from 
		application."user" 
	where
		multi_search_enabled = true;

	select 
		count(*) into _total_adminaccess_users
	from 
		application."user" 
	where
		is_admin = true;

	select 
		count(*) into _total_orgtypesearchenabled_users
	from 
		application."user" 
	where
		org_type_search_enabled = true;

    return query
    select
        'Total Pending Users' AS "Type",
        _total_pending_users AS "Count"
    union
    select
        'Total Authorised Users' AS "Type",
        _total_authorised_users AS "Count"
    union
	select
        'Total Deauthorised Users' AS "Type",
        _total_deauthorised_users AS "Count"
    union
    select
        'Total Request Denied Users' AS "Type",
        _total_requestdenied_users AS "Count"
    union
    select
        'Users Logged on Month' AS "Type",
        _users_loggedon_month AS "Count"
    union
    select
        'Users Logged on Week' AS "Type",
        _users_loggedon_week AS "Count"
    union
    select
        'Total Users with Multi Search Enabled' AS "Type",
        _total_multisearchenabled_users AS "Count"
    union
    select
        'Total Users with Admin Access' AS "Type",
        _total_adminaccess_users AS "Count"
    union
    select
        'Total Users with Organisation Type Search Enabled' AS "Type",
        _total_orgtypesearchenabled_users AS "Count"
    order by 1;
end;
$$ language plpgsql;