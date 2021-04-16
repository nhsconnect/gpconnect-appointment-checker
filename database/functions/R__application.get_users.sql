drop function if exists application.get_users;

create function application.get_users
(
)
returns table
(
	user_id integer, 
	email_address varchar(200), 
	display_name varchar(200), 
	organisation_name varchar(200),
	is_authorised boolean,
	last_logon_date timestamp,
	status text,
	access_level text,
	multi_search_enabled boolean,
	number_of_access_requests bigint,
	is_past_last_logon_threshold boolean
)
as $$
declare last_logon_threshold_highlight timestamp with time zone;
begin

	SELECT
		now() - make_interval(days => cg.last_access_highlight_threshold_in_days) 
	INTO
		last_logon_threshold_highlight
	FROM
		configuration.general cg;
		
	return query
	select
		u.user_id,
		u.email_address,
		u.display_name,
		o.organisation_name,
		u.is_authorised,
		u.last_logon_date,
		CASE WHEN u.is_authorised = True THEN 'Authorised' WHEN u.is_authorised = False THEN 'Unauthorised' END status,
		CASE WHEN u.is_admin = True THEN 'Admin' WHEN u.is_admin = False THEN 'User' END access_level,
		u.multi_search_enabled,
		access_requests.access_requests_count,
		u.last_logon_date <= last_logon_threshold_highlight AS is_past_last_logon_threshold
	from application.user u
	inner join application.organisation o on u.organisation_id = o.organisation_id	
	left outer join 
	(
		select 
			e.user_id, 
			count(e.user_id) AS access_requests_count 
		from
			audit.entry e 
		where 
			entry_type_id = 16
		group by 
			e.user_id
	) as access_requests on u.user_id = access_requests.user_id
	order by u.email_address;	
end;
$$ language plpgsql;