drop function if exists application.add_user_manual;

create function application.add_user_manual
(
	_email_address varchar(200)
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
	last_logon_date timestamp
)
as $$
begin

	_email_address = lower(trim(coalesce(_email_address, '')));

	if exists
	(
		select *
		from application.user u
		where lower(u.email_address) = _email_address
	)
	then
		raise exception '% already exists in table application.user', _email_address;
		return;
	end if;

	insert into application."user" 
	(
		email_address, 
		display_name, 
		organisation_id, 
		is_authorised, 
		added_date, 
		authorised_date,
		last_logon_date
	) 
	values
	(
		_email_address,
		'not-set', 
		-1, 
		true, 
		now(), 
		now(),
		null
	);

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
		u.last_logon_date
	from application.user u
	where u.email_address = _email_address;
	
end;
$$ language plpgsql;