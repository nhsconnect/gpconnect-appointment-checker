drop function if exists application.set_user_status;

create function application.set_user_status
(
	_user_id int,	
	_admin_user_id int,
	_user_account_status_id int,
	_user_session_id int
)
returns table
(
	user_id integer,
	user_session_id integer,
	email_address varchar(200), 
	display_name varchar(200), 
	organisation_id integer,
	user_account_status_id integer,
	multi_search_enabled boolean,
	is_admin boolean
)
as $$
begin
	update application.user
	set
		user_account_status_id = _user_account_status_id,
		authorised_date = case when _user_account_status_id == 2 then now() else null end
	where
		application.user.user_id = _user_id;

	perform audit.add_entry(_user_id, _user_session_id, 10, _user_account_status_id, null, null, null, null, _admin_user_id);

	return query
	select
		u.user_id,
		_user_session_id AS user_session_id,
		u.email_address,
		u.display_name,
		u.organisation_id,
		u.user_account_status_id,
		u.multi_search_enabled,
		u.is_admin
	from application.user u
	where u.user_id = _user_id;
end;
$$ language plpgsql;