do 
$$
declare 
	_email_address varchar(200);
	_display_name varchar(200);
	_ods_code varchar(20);
	_organisation_id integer;
	_user_session_id integer;
	_user_id integer;
begin

	_email_address := 'logoff-test@test.com';
	_display_name := 'Log off test';
	_ods_code := 'X26010';

	select
		o.organisation_id into _organisation_id
	from application.organisation o
	where o.ods_code = _ods_code;

	perform
		user_id,
		user_session_id,
		email_address,
		display_name,
		organisation_id,
		user_account_status_id,
		multi_search_enabled,
		is_admin
	from application.logon_user
	(
		_email_address := _email_address,
		_display_name := _display_name,
		_organisation_id := _organisation_id
	);

	update 
		application.user 
	set 
		user_account_status_id = 2 
	where 
		email_address = _email_address;

	perform
		user_id,
		user_session_id,
		email_address,
		display_name,
		organisation_id,
		user_account_status_id,
		multi_search_enabled,
		is_admin
	from application.logon_user
	(
		_email_address := _email_address,
		_display_name := _display_name,
		_organisation_id := _organisation_id
	);

	select
		u.user_id into _user_id
	from
		application.user u
	where 
		u.email_address = _email_address;

	--perform
	--	user_id,
	--	user_session_id,
	--	email_address,
	--	display_name,
	--	organisation_id,
	--	user_account_status_id,
	--	multi_search_enabled,
	--	is_admin,
	--	status_changed
	--from application.set_user_status
	--(
	--	_user_id := _user_id,
	--	_admin_user_id := _user_id,
	--	_user_account_status_id := 2,
	--	_user_session_id := _user_session_id
	--);

	select
		us.user_session_id into _user_session_id
	from application.user u
	inner join application.user_session us on u.user_id = us.user_id
	where u.email_address = _email_address;

	perform
		*
	from application.logoff_user
	(
		_email_address := _email_address,
		_user_session_id := _user_session_id
	);

end
$$;
