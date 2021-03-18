drop function if exists application.get_users;

create function application.get_users
(
)
returns table
(
	user_id integer, 
	email_address varchar(200), 
	display_name varchar(200), 
	organisation_id integer,
	is_authorised boolean,
	added_date timestamp,
	authorised_date timestamp,
	last_logon_date timestamp,
	multi_search_enabled boolean,
	is_admin boolean
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
		u.organisation_id,
		u.is_authorised,
		u.added_date,
		u.authorised_date,
		u.last_logon_date,
		u.multi_search_enabled,
		u.is_admin
	from application.user u
	order by u.user_id;
	
end;
$$ language plpgsql;