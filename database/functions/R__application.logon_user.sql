drop function if exists application.logon_user;

create function application.logon_user
(
	_email_address varchar(200),
	_display_name varchar(200),
	_organisation_id integer
)
returns table
(
	user_id integer, 
	user_session_id integer, 
	email_address varchar(200), 
	display_name varchar(200), 
	organisation_id integer,
	is_authorised boolean,
	multi_search_enabled boolean,
	is_admin boolean
)
as $$
declare
	_user_id integer;
	_is_authorised boolean;
	_existing_display_name varchar(200);
	_existing_organisation_id integer;
	_user_session_id integer;
	_logon_date timestamp;
	_multi_search_enabled boolean;
	_is_admin boolean;
begin

	--------------------------------------------
	-- clean parameters
	--
	_email_address = trim(coalesce(_email_address, ''));
	_display_name = trim(coalesce(_display_name, ''));
	_logon_date = now();

	if not exists
	(
		select *
		from application.organisation o
		where o.organisation_id = _organisation_id
	)
	then
		raise exception '_organisation_id does not exist in application.organisation';
		return;
	end if;

	--------------------------------------------
	-- find / create user
	--
	select
		u.user_id, 
		u.is_authorised,
		u.display_name,
		u.organisation_id,
		u.multi_search_enabled,
		u.is_admin
	into
		_user_id,
		_is_authorised,
		_existing_display_name,
		_existing_organisation_id,
		_multi_search_enabled,
		_is_admin
	from application.user u
	where lower(u.email_address) = lower(_email_address);

	if (_user_id is null)
	then
		insert into application.user
		(
			email_address,
			display_name,
			organisation_id,
			is_authorised,
			added_date,
			authorised_date,
			last_logon_date,
			multi_search_enabled,
			is_admin
		)
		values
		(
			_email_address,
			_display_name,
			_organisation_id,
			false,
			_logon_date,
			null,
			null,
			false,
			false
		)
		returning
			application.user.user_id, 
			application.user.is_authorised,
			application.user.is_admin,
			application.user.multi_search_enabled
		into 
			_user_id,
			_is_authorised,
			_is_admin,
			_multi_search_enabled;

	end if;

	--------------------------------------------
	-- check if user details have changed
	--
	if (_display_name != _existing_display_name)
	then
		update application.user u
		set display_name = _display_name
		where u.user_id = _user_id;

		-- audit display name changed
		perform
		from audit.add_entry
		(
			_user_id := _user_id,
			_user_session_id := null,
			_entry_type_id := 5,
			_item1 := _existing_display_name,
			_item2 := _display_name
		);
	end if;

	if (_organisation_id != _existing_organisation_id)
	then
		update application.user u
		set organisation_id = _organisation_id
		where u.user_id = _user_id;

		-- audit organisation id changed
		perform
		from audit.add_entry
		(
			_user_id := _user_id,
			_user_session_id := null,
			_entry_type_id := 6,
			_item1 := _existing_organisation_id::text,
			_item2 := _organisation_id::text
		);
	end if;

	--------------------------------------------
	-- if authorised create user session
	--
	if (_is_authorised)
	then
		-- update last logon date
		update application.user u
		set
			last_logon_date = _logon_date
		where u.user_id = _user_id
		and u.is_authorised;

		-- create user session
		insert into application.user_session
		(
			user_id,
			start_time
		)
		values
		(
			_user_id,
			_logon_date
		)
		returning
			user_session.user_session_id 
		into
			_user_session_id;

		-- audit logon success
		perform
		from audit.add_entry
		(
    		_user_id := _user_id,
    		_user_session_id := _user_session_id,
    		_entry_type_id := 1
		);
	else

		-- audit logon failure
		perform
		from audit.add_entry
		(
    		_user_id := _user_id,
    		_user_session_id := _user_session_id,
    		_entry_type_id := 2,
    		_item1 := 'user not authorised'
		);

	end if;

	--------------------------------------------
	-- return user and user session data
	--
	return query
	select
		u.user_id,
		_user_session_id as user_session_id,
		u.email_address,
		u.display_name,
		u.organisation_id,
		_is_authorised as is_authorised,
		_multi_search_enabled as multi_search_enabled,
		_is_admin as is_admin
	from application.user u
	where u.user_id = _user_id;
	
end;
$$ language plpgsql;