drop function if exists application.set_user_status;

create function application.set_user_status
(
	_user_id integer,	
	_admin_user_id integer,
	_user_account_status_id integer,
	_user_session_id integer
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
	is_admin boolean,
	status_changed boolean
)
as $$
declare	_old_user_status character varying (100);
declare	_new_user_status character varying (100);
declare	_status_changed boolean;
begin
	
	select
		uas1.description,
		uas2.description,
		u.user_account_status_id != _user_account_status_id
	from
		application.user_account_status as uas1,
		application.user_account_status as uas2,
		application.user as u
	where
		u.user_id = _user_id		
		and uas1.user_account_status_id = u.user_account_status_id
		and uas2.user_account_status_id = _user_account_status_id	
	into
		_old_user_status, _new_user_status, _status_changed;	

	if(_status_changed)
	begin	
		update application.user
		set
			user_account_status_id = _user_account_status_id,
			authorised_date = case when _user_account_status_id = 2 then now() else null end
		where
			application.user.user_id = _user_id;

		perform audit.add_entry(_user_id, _user_session_id, 10, _old_user_status, _new_user_status, null, null, null, _admin_user_id);
	end;

	return query
	select
		u.user_id,
		_user_session_id AS user_session_id,
		u.email_address,
		u.display_name,
		u.organisation_id,
		u.user_account_status_id,
		u.multi_search_enabled,
		u.is_admin,
		_status_changed AS status_changed
	from application.user u
	where u.user_id = _user_id;
end;
$$ language plpgsql;