drop function if exists application.get_user;

create function application.get_user
(
	_email_address varchar(200)
)
returns table
(
	user_id integer, 
	email_address character varying (200), 
	display_name character varying (200), 
	organisation_name character varying (200),
	user_account_status_id integer,
	last_logon_date timestamp without time zone,
	is_admin boolean,
	multi_search_enabled boolean,
	organisation_id integer,
	org_type_search_enabled boolean
)
as $$
begin			
	return query
	select
		u.user_id,
		u.email_address,
		u.display_name,
		o.organisation_name,
		u.user_account_status_id,
		u.last_logon_date,
		u.is_admin,
		u.multi_search_enabled,
		u.organisation_id,
		u.org_type_search_enabled
	from application.user u
	inner join application.organisation o on u.organisation_id = o.organisation_id	
	where u.email_address = _email_address;
end;
$$ language plpgsql;