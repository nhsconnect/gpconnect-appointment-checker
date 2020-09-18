do 
$$
declare 
	_email_address varchar(200);
	_display_name varchar(200);
	_ods_code varchar(20);
	_organisation_id integer;
begin

	_email_address := 'userisauthorised-test@test.com';
	_display_name := 'User isauthorised test';
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
		is_authorised
	from application.logon_user
	(
		_email_address := _email_address,
		_display_name := _display_name,
		_organisation_id := _organisation_id
	);

	perform 
		*
	from
	application.set_user_isauthorised
	(
		_email_address := _email_address,
		_is_authorised := true
	);

	perform 
		*
	from
	application.set_user_isauthorised
	(
		_email_address := _email_address,
		_is_authorised := true
	);

	perform 
		*
	from
	application.set_user_isauthorised
	(
		_email_address := _email_address,
		_is_authorised := true
	);

end
$$;
