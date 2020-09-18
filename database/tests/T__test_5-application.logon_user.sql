do 
$$
declare 
	_email_address varchar(200);
	_display_name varchar(200);
	_ods_code varchar(20);
	_organisation_id integer;
begin

	_email_address := 'logon-test@test.com';
	_display_name := 'Log on test';
	_ods_code := 'X26010';

	select
		o.organisation_id into _organisation_id
	from application.organisation o
	where o.ods_code = _ods_code;

	-- new user, not authorised
	perform
		user_id,
		user_session_id,
		email_address,
		display_name,
		organisation_id,
		is_authorised
	from application.logon_user
	(
		_email_address := _email_address,
		_display_name := _display_name,
		_organisation_id := _organisation_id
	);

	-- existing user, not authorised
	perform
		user_id,
		user_session_id,
		email_address,
		display_name,
		organisation_id,
		is_authorised
	from application.logon_user
	(
		_email_address := _email_address,
		_display_name := _display_name,
		_organisation_id := _organisation_id
	);

	-- authorise user
	perform 
		*
	from
	application.set_user_isauthorised
	(
		_email_address := _email_address,
		_is_authorised := true
	);

	-- existing user, authorised
	perform
		user_id,
		user_session_id,
		email_address,
		display_name,
		organisation_id,
		is_authorised
	from application.logon_user
	(
		_email_address := _email_address,
		_display_name := _display_name,
		_organisation_id := _organisation_id
	);

end
$$;
