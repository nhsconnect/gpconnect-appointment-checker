drop function if exists application.set_user_is_admin;

create function application.set_user_is_admin
(
	_admin_user_id int,
	_user_id int,
	_is_admin boolean
)
returns void
as $$
begin
	update 
		application.user
	set
		is_admin = _is_admin
	where
		user_id = _user_id
		and user_id != _admin_user_id
		and user_account_status_id = 2;

	perform audit.add_entry(_user_id, 18, (NOT _is_admin)::TEXT, _is_admin::TEXT, null, null, null, _admin_user_id);
end;
$$ language plpgsql;
