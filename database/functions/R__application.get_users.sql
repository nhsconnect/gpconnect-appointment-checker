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
	multi_search_enabled boolean
)
as $$
begin

	--------------------------------------------
	-- return user data
	--
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
		u.multi_search_enabled
	from application.user u
	inner join application.organisation o on u.organisation_id = o.organisation_id
	order by u.email_address;
	
end;
$$ language plpgsql;