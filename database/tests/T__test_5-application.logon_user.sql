-- new user, not authorised
select
	user_id,
	user_session_id,
	email_address,
	display_name,
	organisation_id,
	is_authorised
from application.logon_user
(
	_email_address := 'user.name@test.com',
	_display_name := 'User Name',
	_organisation_id := 1
);

-- existing user, not authorised
select
	user_id,
	user_session_id,
	email_address,
	display_name,
	organisation_id,
	is_authorised
from application.logon_user
(
	_email_address := 'user.name@test.com',
	_display_name := 'User Name',
	_organisation_id := 1
);

-- authorise user
select 
	*
from
application.set_user_isauthorised
(
	_email_address := 'user.name@test.com'
	_is_authorised := true
);

-- existing user, authorised
select
	user_id,
	user_session_id,
	email_address,
	display_name,
	organisation_id,
	is_authorised
from application.logon_user
(
	_email_address := 'user.name@test.com',
	_display_name := 'User Name',
	_organisation_id := 1
);
