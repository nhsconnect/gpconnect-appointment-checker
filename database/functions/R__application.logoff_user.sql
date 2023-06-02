drop function if exists application.logoff_user;

create function application.logoff_user
(
	_user_id integer
)
returns void
as $$
declare	_user_session_id integer;
begin
	select 
		user_session_id into _user_session_id 
	from
		application.user_session 
	where
		user_id = _user_id 
		and end_time is null 
	order by 
		start_time desc 
	limit 1;

	if exists
	(
		select *
		from application.user_session us
		where us.user_session_id = _user_session_id
		and us.end_time is null
	)
	then
		update application.user_session
		set
			end_time = now()
		where user_session_id = _user_session_id;

		-- audit logoff
		perform
		from audit.add_entry
		(
			_user_id := _user_id,
			_entry_type_id := 3
		);
	end if;

end;
$$ language plpgsql;