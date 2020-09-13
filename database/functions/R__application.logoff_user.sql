create or replace function application.logoff_user
(
	_email_address varchar(200),
	_user_session_id integer
)
returns void
as $$
begin

	--------------------------------------------
	-- clean parameters
	--
	_email_address = trim(coalesce(_email_address, ''));

	if not exists
	(
		select *
		from application.user u
		inner join application.user_session s on u.user_id = s.user_id
		where lower(u.email_address) = lower(_email_address)
		and s.user_session_id = _user_session_id
	)
	then
		raise exception '_user_session_id and matching _email_address combination not found';
		return;
	end if;

	--------------------------------------------
	-- end the session
	--
	update application.user_session
	set
		end_time = now()
	where user_session_id = _user_session_id
	and end_time is null;

end;
$$ language plpgsql;