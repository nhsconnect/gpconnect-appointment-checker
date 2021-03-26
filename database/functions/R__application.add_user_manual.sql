drop function if exists application.add_user_manual;

create function application.add_user_manual
(
	_email_address varchar(200),
	_admin_user_id integer default null,
	_user_session_id integer default null
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
	is_admin boolean,
	is_new_user boolean
)
as $$
declare	_user_id integer;
declare _is_new_user boolean := false;
begin
	_email_address = lower(trim(coalesce(_email_address, '')));
	
	select u.user_id into _user_id
	from application.user u
		where lower(u.email_address) = _email_address;
		
	if (_user_id is null) 
	then
		_is_new_user := true;
		insert into application."user" 
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
			'not-set', 
			-1, 
			true, 
			now(), 
			now(),
			null,
			false,
			false
		)
		returning
			application.user.user_id
		into 
			_user_id;

		if (_admin_user_id is not null)
		then
			perform audit.add_entry(_user_id, _user_session_id, 13, null, null, null, _email_address, null, _admin_user_id);
		end if;
	end if;
	
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
		u.is_admin,
		_is_new_user
	from application.user u
	where u.user_id = _user_id;
	
end;
$$ language plpgsql;